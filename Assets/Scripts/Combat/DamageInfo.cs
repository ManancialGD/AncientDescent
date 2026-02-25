using UnityEngine;

public struct DamageInfo
{
    public HealthModule Damager;
    public HealthModule Target;
    public float Damage;
    public float Knockback;
    public float StunTime;

    public DamageInfo(
        HealthModule damager,
        HealthModule target,
        float damage,
        float knockback,
        float stunTime)
    {
        Damager = damager;
        Target = target;
        Damage = damage;
        Knockback = knockback;
        StunTime = stunTime;
    }
}
