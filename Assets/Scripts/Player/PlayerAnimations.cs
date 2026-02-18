using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerController))]
public class PlayerAnimations : NetworkBehaviour
{
    public Transform handHolder;
    public SpriteRenderer pistolSprite;
    public SpriteRenderer handSprite;

    private Animator animator;
    private float aimAngle;
    private PlayerController controller;

    private readonly NetworkVariable<Vector2> networkLookDirection =
     new(
         Vector2.zero,
         NetworkVariableReadPermission.Everyone,
         NetworkVariableWritePermission.Owner
     );

    public override void OnNetworkSpawn()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();

        controller.Health.Damaged += OnDamaged;

        if (!IsOwner)
            networkLookDirection.OnValueChanged += OnLookDirectionChanged;
    }

    public override void OnNetworkDespawn()
    {
        controller.Health.Damaged -= OnDamaged;

        if (!IsOwner)
            networkLookDirection.OnValueChanged -= OnLookDirectionChanged;
    }

    private void Update()
    {
        if (IsOwner)
        { // if owner, update animation based on input
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 aimDir = (mouseWorldPos - (Vector2)handHolder.position).normalized;

            networkLookDirection.Value = aimDir;

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
        else //update movement based on velocity if not owner
            animator.SetBool("isRunning", controller.Movement.Velocity.magnitude > 0.1f);
    }

    private void OnLookDirectionChanged(Vector2 oldValue, Vector2 newValue)
    {
        animator.SetFloat("x", newValue.x);
        animator.SetFloat("y", newValue.y);
    }

    private void OnDamaged(DamageInfo damageInfo)
    {
        int dir = damageInfo.Damager.transform.position.x > transform.position.x ? 1 : 0;
        PlayDamageAnimationClientRpc(dir);
    }

    [ClientRpc]
    private void PlayDamageAnimationClientRpc(int dir)
    {
        animator.SetFloat("hurtDir", dir);
        animator.SetTrigger("hurt");
    }
}
