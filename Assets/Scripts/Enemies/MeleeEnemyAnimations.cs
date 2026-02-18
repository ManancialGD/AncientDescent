using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MeleeEnemy))]
public class MeleeEnemyAnimations : NetworkBehaviour
{
    private Animator animator;
    private MeleeEnemy enemy;

    public override void OnNetworkSpawn()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<MeleeEnemy>();
    }

    private void Update()
    {
        if (!IsServer) return;

        Vector2 dir = enemy.LookDirection;
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
    }

    public void PlayAttackAnimation()
    {
        PlayAttackClientRpc();
    }

    [ClientRpc]
    private void PlayAttackClientRpc()
    {
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
    }
}
