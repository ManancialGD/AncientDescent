using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

    [RequireComponent(typeof(NetworkObject))]
    public class HealthModule : NetworkBehaviour
    {
        [Header("Fallback (Used if no IStatProvider exists)")]
        [SerializeField] private float defaultMaxHealth = 100f;
    
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
    
        private IStatProvider statProvider;
        private float cachedMaxHealth;
        public float MaxHealth =>
            statProvider != null
                ? statProvider.GetStat(StatType.MaxHealth)
                : defaultMaxHealth;
    
        public override void OnNetworkSpawn()
        {
            statProvider = GetComponent<IStatProvider>();
    
            if (IsServer)
            {
                IsDead = false;
    
                cachedMaxHealth = MaxHealth;
                CurrentHealth.Value = cachedMaxHealth;
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
    
        public void Damage(
            HealthModule damager,
            float damageAmount,
            float knockback = 0f,
            float stunTime = 0f)
        {
            if (!IsServer || IsDead) return;
    
            if (damageAmount <= 0f) return;
    
            CurrentHealth.Value -= damageAmount;
    
            Damaged?.Invoke(
                new DamageInfo(damager, this, damageAmount, knockback, stunTime)
            );
    
            onDamaged?.Invoke();
    
            if (CurrentHealth.Value <= 0f)
            {
                CurrentHealth.Value = 0f;
                HandleDeath();
            }
        }
    
        public void Heal(float healAmount)
        {
            if (!IsServer || IsDead) return;
    
            if (healAmount <= 0f) return;
    
            float max = MaxHealth;
    
            CurrentHealth.Value = Mathf.Min(
                CurrentHealth.Value + healAmount,
                max
            );
    
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
        /// Call this on the SERVER whenever MaxHealth may have changed
        /// (items added/removed, buffs applied, etc.)
        /// </summary>
        public void RecalculateMaxHealth(bool preserveHealthPercent = true)
        {
            if (!NetworkManager.Singleton.IsServer || IsDead) return;
    
            float newMax = MaxHealth;
    
            if (Mathf.Approximately(newMax, cachedMaxHealth))
                return;
    
            if (preserveHealthPercent && cachedMaxHealth > 0f)
            {
                float percent = CurrentHealth.Value / cachedMaxHealth;
                CurrentHealth.Value = newMax * percent;
            }
            else
            {
                CurrentHealth.Value = Mathf.Min(CurrentHealth.Value, newMax);
            }
    
            cachedMaxHealth = newMax;
        }
    }
