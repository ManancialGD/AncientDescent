using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkObject))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(HealthModule))]
[RequireComponent(typeof(Animator))]
public class PlayerController : NetworkBehaviour
{
    public float shootCooldown = 0.2f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Animator muzzleFlashAnimator;

    private PlayerControls controls;
    public HealthModule Health { get; private set; }

    public Vector2 MoveInput { get; private set; }

    private float lastShootTime;

    private NetworkVariable<PlayerControlState> playerState =
        new(
            PlayerControlState.Gameplay,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    public PlayerMovement Movement { get; private set; }

    public PlayerControlState PlayerState => playerState.Value;

    public override void OnNetworkSpawn()
    {
        Movement = GetComponent<PlayerMovement>();
        Health = GetComponent<HealthModule>();

        if (!IsOwner) return;

        controls = new PlayerControls();

        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Fire.performed += OnFirePerformed;

        controls.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Fire.performed -= OnFirePerformed;

        controls.Disable();
    }


    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (playerState.Value != PlayerControlState.Gameplay) return;

        SubmitMoveInputServerRpc(MoveInput.normalized);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        MoveInput = Vector2.zero;
    }

    private void OnFirePerformed(InputAction.CallbackContext ctx)
    {
        if (playerState.Value != PlayerControlState.Gameplay)
            return;

        if (Camera.main == null || firePoint == null)
            return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 shootDir = (mouseWorldPos - (Vector2)firePoint.position).normalized;

        if (shootDir == Vector2.zero) return;

        //todo: muzzle flash should be played if the shot is successful
        muzzleFlashAnimator.SetTrigger("fire");
        ShootServerRpc(firePoint.position, shootDir);
    }

    [ClientRpc]
    private void PlayMuzzleClientRpc()
    {
        if (IsOwner) return;
        muzzleFlashAnimator.SetTrigger("fire");
    }

    [ServerRpc]
    private void SubmitMoveInputServerRpc(Vector2 input)
    {
        Movement.ServerMove(input);
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 position, Vector2 direction)
    {
        if (NetworkManager.Singleton.ServerTime.TimeAsFloat < lastShootTime + shootCooldown) return;

        lastShootTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        GameObject proj = Instantiate(projectilePrefab, position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(direction, Health);
        proj.GetComponent<NetworkObject>().Spawn();

        PlayMuzzleClientRpc();
    }

    public void SetPlayerState(PlayerControlState newState)
    {
        if (!IsServer) return;

        playerState.Value = newState;

        if (newState != PlayerControlState.Gameplay)
        {
            MoveInput = Vector2.zero;
        }
    }
}
