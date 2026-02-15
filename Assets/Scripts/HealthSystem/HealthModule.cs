using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class HealthModule : NetworkBehaviour
{
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;

    public NetworkVariable<float> CurrentHealth = new(
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server
    );

    public bool IsDead { get; private set; }

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onDamaged;
    [SerializeField] private UnityEvent onHealed;
    [SerializeField] private UnityEvent onDied;

    public event Action<DamageInfo> Damaged;
    public event Action<float> Healed;
    public event Action<float> HealthChanged;
    public event Action<HealthModule> Died;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            IsDead = false;
            CurrentHealth.Value = MaxHealth;
        }

        CurrentHealth.OnValueChanged += OnHealthChanged;
    }

    public override void OnNetworkDespawn()
    {
        CurrentHealth.OnValueChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(float oldValue, float newValue)
    {
        HealthChanged?.Invoke(newValue);
    }

    public void Damage(HealthModule damager, float damageAmount, float knockback = 0f, float stunTime = 0f)
    {
        if (!NetworkManager.Singleton.IsServer || IsDead) return;

        CurrentHealth.Value -= damageAmount;

        Damaged?.Invoke(
            new DamageInfo(damager, this, damageAmount, knockback, stunTime)
        );
        
        onDamaged?.Invoke();

        if (CurrentHealth.Value <= 0f)
        {
            CurrentHealth.Value = 0f;
            IsDead = true;

            Died?.Invoke(this);
            onDied?.Invoke();
        }
    }

    public void Heal(float healAmount)
    {
        if (!NetworkManager.Singleton.IsServer || IsDead) return;

        CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + healAmount, MaxHealth);

        Healed?.Invoke(healAmount);
        onHealed?.Invoke();
    }
}
