using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(30)]
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerController), typeof(PlayerCombat), typeof(Health))]
public sealed class PlayerDodge : MonoBehaviour
{
    private static readonly int Dodge = Animator.StringToHash("Dodge");
    private static readonly int DodgeSpeed = Animator.StringToHash("DodgeSpeed");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private const float CollisionSkin = 0.015f;

    [SerializeField] private InputActionAsset inputActions;
    [SerializeField, Min(0.02f)] private float duration = 0.20f;
    [SerializeField, Min(0.1f)] private float movementSpeed = 8f;
    [SerializeField, Min(0.1f)] private float distance = 1.6f;
    [SerializeField, Min(0f)] private float cooldown = 0.9f;
    [SerializeField, Min(0f)] private float invulnerability = 0.16f;
    [SerializeField, Min(0.1f)] private float animationSpeed = 1.65f;
    [SerializeField] private Color tint = new(0.78f, 0.89f, 1f, 0.85f);

    private readonly List<RaycastHit2D> hits = new(8);
    private InputAction dodge;
    private PlayerController movement;
    private PlayerCombat combat;
    private Health health;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;
    private ContactFilter2D solids;
    private Color originalTint;
    private Vector2 direction;
    private float nextDodge;
    private float timeLeft;
    private float distanceLeft;
    private bool finishingStep;

    public bool IsDodging { get; private set; }

    private void Awake()
    {
        movement = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        dodge = inputActions.FindAction("Player/Dodge", throwIfNotFound: true).Clone();
        solids.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        solids.useTriggers = false;
    }

    private void OnEnable()
    {
        dodge.Enable();
        health.Damaged += Interrupt;
        health.Died += Interrupt;
    }

    private void OnDisable()
    {
        dodge.Disable();
        health.Damaged -= Interrupt;
        health.Died -= Interrupt;
        Finish(!health.IsDead);
    }

    private void OnDestroy() => dodge?.Dispose();

    private void Update()
    {
        if (!movement.enabled || health.IsDead)
        {
            Finish(!health.IsDead);
            return;
        }
        if (IsDodging || Time.time < nextDodge || !dodge.WasPressedThisFrame() ||
            (movement.IsMovementLocked && !combat.IsAttacking) || !combat.TryCancelForDodge())
            return;

        Vector2 input = movement.ReadMoveInput();
        direction = input.sqrMagnitude > 0.001f ? input.normalized : movement.VisualFacing;
        nextDodge = Time.time + cooldown;
        timeLeft = duration;
        distanceLeft = distance;
        finishingStep = false;
        IsDodging = true;
        AudioManager.Play(SoundCue.Dodge);
        movement.SetMovementLocked(true);
        movement.SetFacing(direction);
        health.GrantInvulnerability(Mathf.Min(invulnerability, duration));
        originalTint = sprite.color;
        sprite.color = originalTint * tint;
        animator.SetFloat(DodgeSpeed, animationSpeed);
        animator.Play(Dodge, 0, 0f);
    }

    private void FixedUpdate()
    {
        if (!IsDodging)
            return;
        if (!movement.enabled || health.IsDead || finishingStep)
        {
            Finish(!health.IsDead);
            return;
        }

        float stepTime = Mathf.Min(Time.fixedDeltaTime, timeLeft);
        float requested = Mathf.Min(movementSpeed * stepTime, distanceLeft);
        float clear = requested;
        body.Cast(direction, solids, hits, requested + CollisionSkin);
        foreach (var hit in hits)
            if (Vector2.Dot(direction, hit.normal) < -0.01f)
                clear = Mathf.Min(clear, Mathf.Max(0f, hit.distance - CollisionSkin));

        if (clear <= 0.001f)
        {
            Finish(true);
            return;
        }

        body.MovePosition(body.position + direction * clear);
        timeLeft -= stepTime;
        distanceLeft -= clear;
        finishingStep = clear < requested || timeLeft <= 0.001f || distanceLeft <= 0.001f;
    }

    private void Interrupt() => Finish(false);

    private void Finish(bool returnToIdle)
    {
        if (!IsDodging)
            return;

        IsDodging = false;
        body.linearVelocity = Vector2.zero;
        movement.SetMovementLocked(false);
        sprite.color = originalTint;
        if (returnToIdle)
            animator.Play(Idle, 0, 0f);
    }
}
