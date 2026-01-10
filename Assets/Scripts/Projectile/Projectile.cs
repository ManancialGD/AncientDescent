using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class Projectile : NetworkBehaviour
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

    // Called on server when projectile is spawned
    public void Initialize(Vector2 dir, HealthModule owner)
    {
        direction = dir.normalized;
        transform.right = direction;
        spawnTime = Time.time;
        ownerHealth = owner;
    }

    private void Update()
    {
        if (!IsServer) return;

        // Move projectile manually
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        // Check for hits using overlap circle
        Collider2D enemyHit = Physics2D.OverlapCircle(transform.position, radius, enemyMask);
        if (enemyHit != null)
        {
            // Damage enemy
            if (enemyHit.TryGetComponent<HealthModule>(out var enemyH))
                enemyH.Damage(ownerHealth, 20f, knockback);

            GetComponent<NetworkObject>().Despawn();
        }

        Collider2D wallHit = Physics2D.OverlapCircle(transform.position, radius, wallMask);

        if (wallHit != null)
            GetComponent<NetworkObject>().Despawn();


        // Lifetime check
        if (Time.time - spawnTime > lifetime)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
#endif
}
