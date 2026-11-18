using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public sealed class LuciferBoss : MonoBehaviour
{
    private enum State { Idle, Chase, Attack, Hurt, PhaseTransition, Death }

    private static readonly int Facing = Animator.StringToHash("Facing");
    private static readonly int IdleAnimation = Animator.StringToHash("Idle");
    private static readonly int WalkAnimation = Animator.StringToHash("Walk");
    private static readonly int HurtAnimation = Animator.StringToHash("Hurt");
    private static readonly int DeathAnimation = Animator.StringToHash("Death");
    private static readonly int[] AttackAnimations =
        { Animator.StringToHash("Attack01"), Animator.StringToHash("Attack02"), Animator.StringToHash("Attack03") };
    private static readonly SoundCue[] AttackSounds =
        { SoundCue.BossAttackOne, SoundCue.BossAttackTwo, SoundCue.BossAttackThree };
    private static readonly float[][] ImpactTimes =
        { new[] { 0.5f }, new[] { 0.3f, 0.6f, 0.9f }, new[] { 0.4f } };
    private static readonly float[] AttackLengths = { 0.7f, 1.3f, 0.7f };
    private static readonly float[] AttackRanges = { 1.2f, 1.75f, 2.15f };

    [SerializeField, Min(0.1f)] private float chaseSpeed = 1.35f;
    [SerializeField, Min(0.1f)] private float recovery = 0.68f;
    [SerializeField] private Material hitFlash;

    private readonly List<Vector2> path = new();
    private Health health;
    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer sprite;
    private Material originalMaterial;
    private Color originalColor;
    private PlayerController player;
    private Health playerHealth;
    private DungeonNavigation navigation;
    private State state;
    private Vector2 facing = Vector2.down;
    private bool engaged, phaseTwo, struck, victoryRaised;
    private int attack, attackCycle, impact;
    private int waypoint;
    private float stateSince, nextAttack, flashUntil, repathAt;

    public Health Health => health;
    public event Action Defeated;

    private void Awake()
    {
        health = GetComponent<Health>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        originalMaterial = sprite.sharedMaterial;
        originalColor = sprite.color;
        health.Damaged += React;
        health.Died += Die;
    }

    public void Engage(PlayerController target, DungeonNavigation graph)
    {
        player = target;
        playerHealth = player.GetComponent<Health>();
        navigation = graph;
        engaged = true;
        nextAttack = Time.time + 0.65f;
        SetState(State.Idle);
    }

    private void FixedUpdate()
    {
        if (!engaged || state == State.Death)
            return;
        if (!player.enabled || playerHealth.IsDead)
        {
            if (state != State.Idle)
                SetState(State.Idle);
            return;
        }

        float elapsed = Time.time - stateSince;
        if (state == State.Attack)
        {
            var animation = animator.GetCurrentAnimatorStateInfo(0);
            if (animation.shortNameHash != AttackAnimations[attack])
                return;

            float playback = animation.normalizedTime * AttackLengths[attack];
            while (impact < ImpactTimes[attack].Length && playback >= ImpactTimes[attack][impact])
            {
                ++impact;
                AudioManager.Play(AttackSounds[attack]);
                if (!struck)
                    struck = TryStrike();
            }
            if (animation.normalizedTime >= 1f)
            {
                nextAttack = Time.time + recovery * (phaseTwo ? .78f : 1f);
                SetState(State.Idle);
            }
            return;
        }
        if (state == State.Hurt || state == State.PhaseTransition)
        {
            if (elapsed >= (state == State.Hurt ? .25f : .65f))
                SetState(State.Idle);
            return;
        }

        Vector2 destination = player.transform.position;
        Vector2 delta = destination - body.position;
        int planned = phaseTwo ? attackCycle % 3 : attackCycle % 2;
        Face(delta);
        bool inRange = delta.magnitude <= AttackRanges[planned] - .15f &&
            CombatGeometry.IsClear(Chest(body.position), Chest(destination));
        if (inRange && Time.time >= nextAttack)
        {
            attack = planned;
            attackCycle++;
            impact = 0;
            struck = false;
            SetState(State.Attack);
        }
        else if (!inRange)
        {
            SetState(State.Chase);
            Vector2 direction = ChaseDirection(destination);
            body.linearVelocity = direction * chaseSpeed * (phaseTwo ? 1.17f : 1f);
        }
        else
            SetState(State.Idle);
    }

    private Vector2 ChaseDirection(Vector2 destination)
    {
        if (navigation == null || navigation.DirectPath(body.position, destination))
            return (destination - body.position).normalized;
        if (Time.time >= repathAt)
        {
            navigation.FindPath(body.position, destination, path);
            waypoint = 0;
            repathAt = Time.time + .28f;
        }
        while (waypoint < path.Count && Vector2.Distance(body.position, path[waypoint]) < .09f)
            waypoint++;
        return waypoint < path.Count ? (path[waypoint] - body.position).normalized : Vector2.zero;
    }

    private static Vector2 Chest(Vector2 position) => position + Vector2.up * .24f;

    private bool TryStrike()
    {
        Vector2 destination = player.transform.position;
        Vector2 delta = destination - body.position;
        float cone = attack == 0 ? .6f : attack == 1 ? .12f : .35f;
        if (delta.sqrMagnitude > AttackRanges[attack] * AttackRanges[attack] ||
            Vector2.Dot(facing, delta.normalized) < cone ||
            !CombatGeometry.IsClear(Chest(body.position), Chest(destination)))
            return false;
        return playerHealth.TryDamage(attack == 2 ? 2 : 1);
    }

    private void Face(Vector2 delta)
    {
        if (delta.sqrMagnitude < .001f)
            return;
        int direction = Mathf.Abs(delta.x) > Mathf.Abs(delta.y) ? (delta.x < 0 ? 1 : 3) : (delta.y < 0 ? 0 : 2);
        facing = direction switch { 0 => Vector2.down, 1 => Vector2.left, 2 => Vector2.up, _ => Vector2.right };
        animator.SetFloat(Facing, direction);
    }

    private void SetState(State next)
    {
        if (state == next)
            return;
        state = next;
        stateSince = Time.time;
        body.linearVelocity = Vector2.zero;
        animator.Play(next switch
        {
            State.Chase => WalkAnimation,
            State.Attack => AttackAnimations[attack],
            State.Hurt => HurtAnimation,
            State.Death => DeathAnimation,
            _ => IdleAnimation
        }, 0, 0f);
    }

    private void React()
    {
        flashUntil = Time.time + .09f;
        AudioManager.Play(SoundCue.BossHurt);
        if (!engaged)
            return;
        if (!phaseTwo && health.Current * 2 < health.Maximum)
        {
            phaseTwo = true;
            attackCycle = 2;
            health.GrantInvulnerability(.65f);
            nextAttack = Time.time + .80f;
            SetState(State.PhaseTransition);
            AudioManager.Play(SoundCue.BossPhase);
        }
        else if (state is State.Idle or State.Chase && Time.time >= nextAttack)
        {
            nextAttack = Time.time + .28f;
            SetState(State.Hurt);
        }
    }

    private void Die()
    {
        flashUntil = Time.time + .09f;
        SetState(State.Death);
        body.simulated = false;
        AudioManager.BossDied();
    }

    private void LateUpdate()
    {
        bool flashing = Time.time < flashUntil;
        sprite.sharedMaterial = flashing && hitFlash != null ? hitFlash : originalMaterial;
        float elapsed = Time.time - stateSince;
        bool warning = state == State.Attack && impact < ImpactTimes[attack].Length &&
            ImpactTimes[attack][impact] - elapsed <= .50f;
        sprite.color = !flashing && (warning || state == State.PhaseTransition)
            ? Color.Lerp(originalColor, originalColor * new Color(1f,.65f,.22f,1f),
                .78f + .22f * Mathf.Sin(elapsed * 15f)) : originalColor;
        if (state == State.Death && !victoryRaised && elapsed >= .90f &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            victoryRaised = true;
            Defeated?.Invoke();
        }
    }

    private void OnDestroy()
    {
        health.Damaged -= React;
        health.Died -= Die;
    }
}
