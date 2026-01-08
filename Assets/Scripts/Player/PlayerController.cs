using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.iOS;

[RequireComponent(typeof(Rigidbody2D), typeof(NetworkObject))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(HealthModule))]
public class PlayerController : NetworkBehaviour
{
    public float shootCooldown = 0.2f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private PlayerControls controls;
    private HealthModule health;

    private Vector2 moveInput;

    private float lastShootTime;

    public NetworkVariable<PlayerControlState> playerState =
        new(
            PlayerControlState.Gameplay,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

    private PlayerMovement movement;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<HealthModule>();

        controls = new PlayerControls();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

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

        SubmitMoveInputServerRpc(moveInput.normalized);
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
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

        ShootServerRpc(firePoint.position, shootDir);
    }

    [ServerRpc]
    private void SubmitMoveInputServerRpc(Vector2 input)
    {
        movement.ServerMove(input);
    }

    [ServerRpc]
    private void ShootServerRpc(Vector2 position, Vector2 direction)
    {
        if (Time.time < lastShootTime + shootCooldown) return;

        lastShootTime = Time.time;
        GameObject proj = Instantiate(projectilePrefab, position, Quaternion.identity);
        proj.GetComponent<Projectile>().Initialize(direction, health);
        proj.GetComponent<NetworkObject>().Spawn();
    }

    public void SetPlayerState(PlayerControlState newState)
    {
        if (!IsServer) return;
        playerState.Value = newState;
    }
}
