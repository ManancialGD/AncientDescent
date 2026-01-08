using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float friction = 6f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    public void ServerMoveTowards(Vector2 targetPosition)
    {
        if (!IsServer) return;

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;

        Vector2 impulse = Accelerate(dir, moveSpeed, acceleration);
        rb.AddForce(impulse, ForceMode2D.Impulse);
    }
    
    private void FixedUpdate()
    {
        if (!IsServer) return;
        ApplyFriction();
    }

    private Vector2 Accelerate(Vector2 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector2.Dot(rb.linearVelocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0f)
            return Vector2.zero;

        float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
        accelSpeed = Mathf.Min(accelSpeed, addSpeed);

        return wishDir * accelSpeed;
    }

    private void ApplyFriction()
    {
        float speed = rb.linearVelocity.magnitude;
        if (speed < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float drop = speed * friction * Time.fixedDeltaTime;
        rb.linearVelocity *= Mathf.Max(speed - drop, 0) / speed;
    }

    public void ApplyKnockback(Vector2 dir, float force)
    {
        if (!IsServer) return;
        rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
    }
}
