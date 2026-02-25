using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class HealthModule : MonoBehaviour
{
    [Header("Fallback (Used if no IStatProvider exists)")]
    [SerializeField] private float defaultMaxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onDamaged;
    [SerializeField] private UnityEvent onHealed;
    [SerializeField] private UnityEvent onDied;

    public event Action<DamageInfo> Damaged;
    public event Action<float> Healed;
    public event Action<HealthModule> Died;

    private IStatProvider statProvider;
    private float cachedMaxHealth;

    public float MaxHealth =>
        statProvider != null
            ? statProvider.GetStat(StatType.MaxHealth)
            : defaultMaxHealth;


    private void Awake()
    {
        statProvider = GetComponent<IStatProvider>();
        IsDead = false;
        cachedMaxHealth = MaxHealth;
        CurrentHealth = cachedMaxHealth;
    }

    public void Damage(
        HealthModule damager,
        float damageAmount,
        float knockback = 0f,
        float stunTime = 0f)
    {
        if (IsDead || damageAmount <= 0f) return;

        CurrentHealth -= damageAmount;

        Damaged?.Invoke(
            new DamageInfo(damager, this, damageAmount, knockback, stunTime)
        );

        onDamaged?.Invoke();

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            HandleDeath();
        }
    }

    public void Heal(float healAmount)
    {
        if (IsDead || healAmount <= 0f) return;

        float max = MaxHealth;
        CurrentHealth = Mathf.Min(CurrentHealth + healAmount, max);
        Healed?.Invoke(healAmount);
        onHealed?.Invoke();
    }

    private void HandleDeath()
    {
        if (IsDead) return;
        IsDead = true;
        Died?.Invoke(this);
        onDied?.Invoke();
    }

    /// <summary>
    /// Call this whenever MaxHealth may have changed (items added/removed, buffs applied, etc.)
    /// </summary>
    public void RecalculateMaxHealth(bool preserveHealthPercent = true)
    {
        if (IsDead) return;

        float newMax = MaxHealth;

        if (Mathf.Approximately(newMax, cachedMaxHealth))
            return;

        if (preserveHealthPercent && cachedMaxHealth > 0f)
        {
            float percent = CurrentHealth / cachedMaxHealth;
            CurrentHealth = newMax * percent;
        }
        else
        {
            CurrentHealth = Mathf.Min(CurrentHealth, newMax);
        }

        cachedMaxHealth = newMax;
    }
}
