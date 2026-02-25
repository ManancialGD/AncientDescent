using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float knockback = 60f;
    public float speed = 10f;
    public float radius = 0.2f;
    public float lifetime = 2f;
    public LayerMask enemyMask;
    public LayerMask wallMask;

    private Vector2 direction;
    private float spawnTime;
    private HealthModule ownerHealth;

    public void Initialize(Vector2 dir, HealthModule owner)
    {
        direction = dir.normalized;
        transform.right = direction;
        spawnTime = Time.time;
        ownerHealth = owner;
    }

    private void Update()
    {
        // Move projectile
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Check for hits
        Collider2D enemyHit = Physics2D.OverlapCircle(transform.position, radius, enemyMask);
        if (enemyHit != null)
        {
            if (enemyHit.TryGetComponent<HealthModule>(out var enemyH))
                enemyH.Damage(ownerHealth, 20f, knockback);

            Destroy(gameObject);
        }

        Collider2D wallHit = Physics2D.OverlapCircle(transform.position, radius, wallMask);
        if (wallHit != null)
            Destroy(gameObject);

        if (Time.time - spawnTime > lifetime)
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
