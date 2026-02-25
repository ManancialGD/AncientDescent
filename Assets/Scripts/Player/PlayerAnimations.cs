using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimations : MonoBehaviour
{
    public Transform handHolder;
    public SpriteRenderer pistolSprite;
    public SpriteRenderer handSprite;

    private Animator animator;
    private float aimAngle;
    private PlayerController controller;
    private Vector2 lookDirection = Vector2.zero;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        controller.Health.Damaged += OnDamaged;
    }

    private void OnDisable()
    {
        controller.Health.Damaged -= OnDamaged;
    }

    private void Update()
    {
        // Update based on input
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 aimDir = (mouseWorldPos - (Vector2)handHolder.position).normalized;

        lookDirection = aimDir;

        animator.SetFloat("x", aimDir.x);
        animator.SetFloat("y", aimDir.y);
        animator.SetBool("isRunning", controller.MoveInput.magnitude > 0.1f);

        if (controller.PlayerState != PlayerControlState.Gameplay) return;

        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        aimAngle = angle;

        handHolder.rotation = Quaternion.Euler(0f, 0f, aimAngle);

        if (Mathf.Abs(aimAngle) > 90f)
            pistolSprite.transform.localRotation = Quaternion.Euler(180f, 0f, 0);
        else
            pistolSprite.transform.localRotation = Quaternion.Euler(0f, 0, 0);

        if (aimAngle > 0)
        {
            handSprite.sortingOrder = -1;
            pistolSprite.sortingOrder = -2;
        }
        else
        {
            handSprite.sortingOrder = 2;
            pistolSprite.sortingOrder = 1;
        }
    }

    private void OnDamaged(DamageInfo damageInfo)
    {
        int dir = damageInfo.Damager.transform.position.x > transform.position.x ? 1 : 0;
        animator.SetFloat("hurtDir", dir);
        animator.SetTrigger("hurt");
    }
}
