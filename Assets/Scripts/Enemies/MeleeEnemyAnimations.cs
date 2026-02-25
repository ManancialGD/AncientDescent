using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(MeleeEnemy))]
public class MeleeEnemyAnimations : MonoBehaviour
{
    private Animator animator;
    private MeleeEnemy enemy;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        enemy = GetComponent<MeleeEnemy>();
    }

    private void Update()
    {
        Vector2 dir = enemy.LookDirection;
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
    }

    public void PlayAttackAnimation()
    {
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
    }
}
