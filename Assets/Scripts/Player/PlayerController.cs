using AncientDescent.CameraUtil;
using AncientDescent.Combat;
using AncientDescent.Input;
using AncientDescent.Stats;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AncientDescent.Player
{    
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerMovement))]
    [RequireComponent(typeof(HealthModule))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        public HealthModule Health
        {
            get
            {
                health ??= GetComponent<HealthModule>();
                return health;
            }
        }
    
        public PlayerMovement Movement
        {
            get
            {
                movement ??= GetComponent<PlayerMovement>();
                return movement;
            }
        }
        public Vector2 MoveInput { get; private set; }
        public PlayerControlState PlayerState => playerState;
    
        private PlayerStats stats;
        private float ShootRate => stats.GetStat(StatType.ShootRate);
        public GameObject projectilePrefab;
        public Transform firePoint;
        public Animator muzzleFlashAnimator;
        private PlayerControls controls;
        private PlayerInteraction interaction;
        private float lastShootTime;
        private PlayerControlState playerState = PlayerControlState.Gameplay;
        private bool isFiring;
        private PlayerMovement movement;
        private HealthModule health;
    
        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            movement = GetComponent<PlayerMovement>();
            health = GetComponent<HealthModule>();
            interaction = GetComponent<PlayerInteraction>();
    
            controls = new PlayerControls();
    
            controls.Player.Move.performed += OnMovePerformed;
            controls.Player.Move.canceled += OnMoveCanceled;
            controls.Player.Fire.started += OnFireStarted;
            controls.Player.Fire.canceled += OnFireCanceled;
            controls.Player.Interact.performed += OnInteractPerformed;
    
            controls.Enable();
    
            Camera.main.GetComponent<CameraFollow>()?.SetFollowTarget(transform);
        }
    
        private void OnDestroy()
        {
            controls.Player.Move.performed -= OnMovePerformed;
            controls.Player.Move.canceled -= OnMoveCanceled;
            controls.Player.Fire.started -= OnFireStarted;
            controls.Player.Fire.canceled -= OnFireCanceled;
            controls.Player.Interact.performed -= OnInteractPerformed;
    
            controls.Disable();
        }
    
        private void FixedUpdate()
        {
            if (playerState != PlayerControlState.Gameplay) return;
            Movement.ServerMove(MoveInput.normalized);
        }
    
        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<Vector2>();
        }
    
        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            MoveInput = Vector2.zero;
        }
    
        private void OnFireStarted(InputAction.CallbackContext ctx)
        {
            isFiring = true;
        }
    
        private void OnFireCanceled(InputAction.CallbackContext ctx)
        {
            isFiring = false;
        }
    
        private void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            interaction?.OnInteractPerformed(ctx);
        }
    
        private void Update()
        {
            if (!isFiring || playerState != PlayerControlState.Gameplay) return;
            TryShoot();
        }
    
        private void TryShoot()
        {
            float currentTime = Time.time;
    
            if (currentTime < lastShootTime + ShootRate)
                return;
    
            if (Camera.main == null || firePoint == null)
                return;
    
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 shootDir = (mouseWorldPos - (Vector2)firePoint.position).normalized;
    
            if (shootDir == Vector2.zero)
                return;
    
            lastShootTime = currentTime;
    
            Shoot(shootDir);
        }
    
        private void Shoot(Vector2 direction)
        {
            GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>().Initialize(direction, Health);
    
            if (muzzleFlashAnimator != null)
                muzzleFlashAnimator.SetTrigger("fire");
        }
    
        public void SetPlayerState(PlayerControlState newState)
        {
            playerState = newState;
    
            if (newState != PlayerControlState.Gameplay)
            {
                MoveInput = Vector2.zero;
            }
        }
    }

}
