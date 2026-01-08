using UnityEngine;

public struct DamageInfo
{
    public HealthModule Damager;
    public HealthModule Target;
    public float Damage;
    public Vector2 HitPosition;
    public float Knockback;
    public float StunTime;

    public DamageInfo(
        HealthModule damager,
        HealthModule target,
        float damage,
        Vector2 hitPosition,
        float knockback,
        float stunTime)
    {
        Damager = damager;
        Target = target;
        Damage = damage;
        HitPosition = hitPosition;
        Knockback = knockback;
        StunTime = stunTime;
    }
}
