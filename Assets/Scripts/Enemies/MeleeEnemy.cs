using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(HealthModule))]
[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(MeleeEnemyAnimations))]
[RequireComponent(typeof(NetworkObject))]
public class MeleeEnemy : NetworkBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackImpulse = 60;
    public float attackRadius = 1.4f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float attackRecoveryTime = 1.0f;
    public float knockback = 30f;
    [SerializeField] private LayerMask playerLayer;

    private float lastAttackTime;
    private bool isAttacking;
    private bool isRecovering;

    private Transform targetPlayer;
    private HealthModule health;
    private EnemyMovement movement;
    private MeleeEnemyAnimations animations;

    public Vector2 LookDirection =>
        targetPlayer != null
            ? (targetPlayer.position - transform.position).normalized
            : Vector2.zero;


    private void Update()
    {
        if (!IsServer) return;
        if (health.IsDead) return;

        AcquireTarget();
        if (targetPlayer == null) return;

        if (isAttacking || isRecovering)
            return;

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance > attackRange)
        {
            movement.ServerMoveTowards(targetPlayer.position);
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        movement.Rb.AddForce(LookDirection.normalized * attackImpulse, ForceMode2D.Impulse);

        animations.PlayAttackAnimation();
    }

    // === ANIMATION EVENT (HIT FRAME) ===
    public void ApplyAttackHit()
    {
        if (!IsServer || !isAttacking)
            return;

        isAttacking = false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position + (Vector3)(LookDirection * attackRange),
            attackRadius,
            playerLayer
        );

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out HealthModule playerHealth))
                continue;

            playerHealth.Damage(health, attackDamage, knockback);

            if (knockback > 0f &&
                playerHealth.TryGetComponent(out PlayerMovement playerMovement))
            {
                Vector2 dir = (playerMovement.transform.position - transform.position).normalized;
                playerMovement.ApplyKnockback(dir, knockback);
            }
        }

        isRecovering = true;
    }

    // === ANIMATION EVENT (LAST FRAME) ===
    public void EndAttack()
    {
        if (!IsServer) return;

        isRecovering = false;
    }

    private void AcquireTarget()
    {
        if (targetPlayer != null) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                targetPlayer = client.PlayerObject.transform;
                break;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        health = GetComponent<HealthModule>();
        movement = GetComponent<EnemyMovement>();
        animations = GetComponent<MeleeEnemyAnimations>();

        if (IsServer)
        {
            health.Died += OnDied;
            health.Damaged += OnDamaged;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            health.Died -= OnDied;
            health.Damaged -= OnDamaged;
        }
    }

    private void OnDied(HealthModule _)
    {
        GetComponent<NetworkObject>().Despawn();
    }

    private void OnDamaged(DamageInfo info)
    {
        Vector2 knockBackDir = (transform.position - info.Damager.transform.position).normalized;
        movement.Rb.AddForce(knockBackDir * info.Knockback, ForceMode2D.Impulse);
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.red;
        Vector3 dir = LookDirection == Vector2.zero ? transform.right : LookDirection;
        Gizmos.DrawWireSphere(transform.position + (dir * attackRange), attackRadius);
    }
#endif
}
