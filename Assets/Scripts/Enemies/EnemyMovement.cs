using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float friction = 6f;

    public Rigidbody2D Rb { get; private set; }

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = 0;
        Rb.freezeRotation = true;
    }

    public void ServerMoveTowards(Vector2 targetPosition)
    {
        if (!IsServer) return;

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;

        Vector2 impulse = Accelerate(dir, moveSpeed, acceleration);
        Rb.AddForce(impulse, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        if (!IsServer) return;
        ApplyFriction();
    }

    private Vector2 Accelerate(Vector2 wishDir, float wishSpeed, float accel)
    {
        float currentSpeed = Vector2.Dot(Rb.linearVelocity, wishDir);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0f)
            return Vector2.zero;

        float accelSpeed = accel * Time.fixedDeltaTime * wishSpeed;
        accelSpeed = Mathf.Min(accelSpeed, addSpeed);

        return wishDir * accelSpeed;
    }

    private void ApplyFriction()
    {
        float speed = Rb.linearVelocity.magnitude;
        if (speed < 0.01f)
        {
            Rb.linearVelocity = Vector2.zero;
            return;
        }

        float drop = speed * friction * Time.fixedDeltaTime;
        Rb.linearVelocity *= Mathf.Max(speed - drop, 0) / speed;
    }

    public void ApplyKnockback(Vector2 dir, float force)
    {
        if (!IsServer) return;
        Rb.AddForce(dir.normalized * force, ForceMode2D.Impulse);
    }
}
