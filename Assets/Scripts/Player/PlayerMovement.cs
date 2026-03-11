using AncientDescent.Stats;
using UnityEngine;

namespace AncientDescent.Player
{    
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerStats stats;
    
        private float MaxSpeed => stats.GetStat(StatType.MoveSpeed);
        private float Acceleration => stats.GetStat(StatType.Acceleration);
        private float Friction => stats.GetStat(StatType.Friction);
    
        public bool IsMoving => RB.linearVelocity.sqrMagnitude > 0.01f;
        public Vector2 Velocity => RB.linearVelocity;
    
        public Rigidbody2D RB { get; private set; }
    
        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
    
            RB = GetComponent<Rigidbody2D>();
            RB.gravityScale = 0;
            RB.freezeRotation = true;
        }
    
        public void ServerMove(Vector2 movementInput)
        {
            movementInput = movementInput.normalized;
    
            Vector2 accumulatedForce = Vector2.zero;
    
            if (movementInput.sqrMagnitude > 0.001f)
            {
                accumulatedForce += Accelerate(
                    movementInput,
                    MaxSpeed,
                    Acceleration
                );
            }
    
            ApplyFriction();
            ApplyVelocity(accumulatedForce);
        }
    
        private Vector2 Accelerate(Vector2 wishDir, float wishSpeed, float accel)
        {
            float currentSpeed = Vector2.Dot(RB.linearVelocity, wishDir);
            float addSpeed = wishSpeed - currentSpeed;
    
            if (addSpeed <= 0f)
                return Vector2.zero;
    
            float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
            accelSpeed = Mathf.Min(accelSpeed, addSpeed);
    
            return wishDir * accelSpeed;
        }
    
        private void ApplyFriction()
        {
            Vector2 velocity = RB.linearVelocity;
            float speed = velocity.magnitude;
    
            if (speed < 0.01f)
            {
                RB.linearVelocity = Vector2.zero;
                return;
            }
    
            float drop = speed * Friction * Time.fixedDeltaTime;
            float newSpeed = Mathf.Max(speed - drop, 0);
    
            RB.linearVelocity = velocity * (newSpeed / speed);
        }
    
        private void ApplyVelocity(Vector2 impulse)
        {
            if (impulse.sqrMagnitude > 0.001f)
            {
                RB.AddForce(impulse, ForceMode2D.Impulse);
            }
        }
    
        public void ApplyKnockback(Vector2 direction, float force)
        {
            RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        }
    }
}
