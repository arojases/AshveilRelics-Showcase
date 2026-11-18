using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public sealed class SkeletonBrain : MonoBehaviour
{
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly Color WarningOrange = new(1f, 0.70f, 0.24f);
    private const float HitFlashDuration = 0.09f;
    private static Material hitFlashMaterial;

    [SerializeField] private PlayerController target;
    [SerializeField] private DungeonNavigation navigation;
    [SerializeField] private bool artFacesLeft;
    [SerializeField, Min(0.1f)] private float speed = 1.45f;
    [SerializeField, Min(0.1f)] private float detectionRadius = 3.4f;
    [SerializeField, Min(0.1f)] private float loseRadius = 5f;
    [SerializeField, Min(0.1f)] private float attackRange = 0.78f;
    [SerializeField, Min(0.01f)] private float attackWindup = 0.44f;
    [SerializeField, Min(0)] private float attackCooldown = 1.35f;
    [SerializeField, Min(1)] private int damage = 1;
    [SerializeField, Min(0)] private float corpseDuration = 0.85f;

    private readonly List<Vector2> path = new();
    private Health health;
    private Health targetHealth;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;
    private Color originalColor;
    private Material originalMaterial;
    private float hitFlashUntil;
    private bool showingHitFlash;
    private bool pursuing;
    private bool isOrc;
    private bool windingUp;
    private float strikeAt;
    private float nextAttack;
    private float staggerUntil;
    private float refreshPathAt;
    private int waypoint;

    public float WeaponWindup => windingUp ? Mathf.Clamp01(1f - (strikeAt - Time.time) / attackWindup) : -1f;
    public float LastWeaponStrike { get; private set; } = float.NegativeInfinity;
    public bool WeaponBehindBody { get; private set; }

    private void Awake()
    {
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isOrc = AudioManager.IsOrc(animator.runtimeAnimatorController);
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (target == null)
            target = FindFirstObjectByType<PlayerController>();
        if (target != null)
            targetHealth = target.GetComponent<Health>();
    }

    private void OnEnable()
    {
        originalColor = sprite.color;
        originalMaterial = sprite.sharedMaterial;
        if (hitFlashMaterial == null)
            hitFlashMaterial = Resources.Load<Material>("EnemyHitFlash");
        health.Damaged += Stagger;
        health.Died += Die;
    }

    private void OnDisable()
    {
        health.Damaged -= Stagger;
        health.Died -= Die;
        hitFlashUntil = 0f;
        RestoreVisual();
    }

    private void FixedUpdate()
    {
        if (health.IsDead)
            return;
        if (targetHealth == null || targetHealth.IsDead || !target.enabled)
        {
            windingUp = false;
            Move(Vector2.zero);
            return;
        }
        if (Time.time < staggerUntil)
            return;

        Vector2 destination = target.transform.position;
        Vector2 delta = destination - body.position;
        float distance = delta.magnitude;
        if (distance > Mathf.Max(detectionRadius, loseRadius))
        {
            pursuing = false;
            windingUp = false;
        }
        else if (distance <= detectionRadius)
        {
            pursuing = true;
        }
        bool inRange = distance <= attackRange &&
            CombatGeometry.IsClear(body.position + Vector2.up * 0.2f, destination + Vector2.up * 0.2f);
        if (windingUp)
        {
            if (Time.time >= strikeAt)
            {
                windingUp = false;
                nextAttack = Time.time + attackCooldown;
                LastWeaponStrike = Time.time;
                AudioManager.Play(isOrc ? SoundCue.OrcAttack : SoundCue.SkeletonAttack);
                if (inRange)
                    targetHealth.TryDamage(damage);
            }
            return;
        }

        if (!pursuing || inRange)
        {
            Move(Vector2.zero);
            if (pursuing && Time.time >= nextAttack)
            {
                Face(delta);
                windingUp = true;
                strikeAt = Time.time + attackWindup;
            }
            return;
        }
        Move(GetChaseVelocity(destination));
    }

    private Vector2 GetChaseVelocity(Vector2 destination)
    {
        if (navigation == null || navigation.DirectPath(body.position, destination))
            return (destination - body.position).normalized * speed;

        if (Time.time >= refreshPathAt)
        {
            navigation.FindPath(body.position, destination, path);
            waypoint = 0;
            refreshPathAt = Time.time + 0.3f;
        }
        while (waypoint < path.Count && Vector2.Distance(body.position, path[waypoint]) < 0.09f)
            waypoint++;
        return waypoint < path.Count ? (path[waypoint] - body.position).normalized * speed : Vector2.zero;
    }

    private void Face(Vector2 direction)
    {
        float x = direction.x;
        if (Mathf.Abs(x) > 0.01f)
            sprite.flipX = artFacesLeft ? x > 0 : x < 0;
        if (direction.sqrMagnitude > .0001f)
            WeaponBehindBody = direction.y > Mathf.Abs(x);
    }

    private void Move(Vector2 velocity)
    {
        body.linearVelocity = velocity;
        animator.SetBool(IsMoving, velocity.sqrMagnitude > 0.01f);
        Face(velocity);
    }

    private void Stagger()
    {
        windingUp = false;
        staggerUntil = Time.time + 0.16f;
        nextAttack = Mathf.Max(nextAttack, staggerUntil + 0.12f);
        hitFlashUntil = Time.time + HitFlashDuration;
        Move(Vector2.zero);
        AudioManager.Play(isOrc ? SoundCue.OrcHurt : SoundCue.SkeletonHurt);
    }

    private void Die()
    {
        Move(Vector2.zero);
        body.simulated = false;
        windingUp = false;
        hitFlashUntil = Time.time + HitFlashDuration;
        animator.Play(Death, 0, 0f);
        AudioManager.Play(isOrc ? SoundCue.OrcDeath : SoundCue.SkeletonDeath);
        Destroy(gameObject, corpseDuration);
    }

    private void LateUpdate()
    {
        if (Time.time < hitFlashUntil && hitFlashMaterial != null)
        {
            if (!showingHitFlash)
            {
                sprite.color = originalColor;
                sprite.sharedMaterial = hitFlashMaterial;
                showingHitFlash = true;
            }
            return;
        }

        if (showingHitFlash)
            RestoreVisual();

        float progress = windingUp ? WeaponWindup : 0f;
        float pulse = 0.7f + 0.3f * Mathf.Sin(progress * Mathf.PI);
        var color = windingUp ? Color.Lerp(originalColor, originalColor * WarningOrange, pulse) : originalColor;
        color.a = originalColor.a;
        sprite.color = color;
    }

    private void RestoreVisual()
    {
        if (showingHitFlash)
            sprite.sharedMaterial = originalMaterial;
        showingHitFlash = false;
        sprite.color = originalColor;
    }
}
