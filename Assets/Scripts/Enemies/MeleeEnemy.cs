using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(HealthModule), typeof(EnemyMovement))]
[RequireComponent(typeof(NetworkObject))]
public class MeleeEnemy : NetworkBehaviour
{
    [Header("Combat Settings")]
    public float attackRange = 1.2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1.5f;
    public float attackRecoveryTime = 1.0f;
    public float knockback = 30f;

    private float lastAttackTime;
    private bool isRecovering;
    private float recoveryEndTime;

    private Transform targetPlayer;
    private HealthModule health;
    private EnemyMovement movement;

    private void Awake()
    {
        health = GetComponent<HealthModule>();
        movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        if (!IsServer) return;
        if (health.IsDead) return;

        AcquireTarget();
        if (targetPlayer == null) return;

        if (isRecovering)
        {
            if (Time.time >= recoveryEndTime)
                isRecovering = false;

            return; // stop moving and attacking
        }

        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        if (distance > attackRange)
        {
            movement.ServerMoveTowards(targetPlayer.position);
        }
        else
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        if (targetPlayer.TryGetComponent<HealthModule>(out var playerHealth))
        {
            playerHealth.Damage(health, attackDamage, knockback);
        }

        if (knockback > 0f && targetPlayer.TryGetComponent(out PlayerMovement playerM))
        {
            Vector2 dir = ((Vector2)targetPlayer.position - (Vector2)transform.position).normalized;
            playerM.ApplyKnockback(dir, knockback);
        }

        isRecovering = true;
        recoveryEndTime = Time.time + attackRecoveryTime;
    }

    private void AcquireTarget()
    {
        if (targetPlayer != null) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObj = client.PlayerObject;
            if (playerObj != null)
            {
                targetPlayer = playerObj.transform;
                break;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        health.Died += OnDied;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        health.Died -= OnDied;
    }

    private void OnDied(HealthModule _)
    {
        GetComponent<NetworkObject>()?.Despawn();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isRecovering ? Color.yellow : Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
