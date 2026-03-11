using System.Collections;
using AncientDescent.Combat;
using UnityEngine;

namespace AncientDescent.Player
{
    public class PlayerHPBar : MonoBehaviour
    {
        [SerializeField] private HealthModule healthModule;

        [SerializeField] private RectTransform fill;

        private void Awake()
        {
            if (healthModule == null)
            {
                PlayerController player = FindAnyObjectByType<PlayerController>();

                if (player != null)
                    healthModule = player.GetComponent<HealthModule>();
            }
        }

        private void OnEnable()
        {
            if (healthModule == null)
                return;

            healthModule.Damaged += OnDamaged;
            healthModule.Healed += OnHealed;
            healthModule.MaxHealthChanged += OnMaxHealthChanged;
        }

        private void OnDisable()
        {
            if (healthModule == null)
                return;

            healthModule.Damaged -= OnDamaged;
            healthModule.Healed -= OnHealed;
            healthModule.MaxHealthChanged -= OnMaxHealthChanged;
        }

        private void Start()
        {
            UpdateFill();
        }

        private void OnDamaged(DamageInfo damageInfo)
        {
            UpdateFill();
        }

        private void OnHealed(float _)
        {
            UpdateFill();
        }

        private void OnMaxHealthChanged(HealthModule _)
        {
            UpdateFill();
        }

        private void UpdateFill()
        {
            if (healthModule == null || fill == null)
                return;

            if (healthModule.MaxHealth <= 0)
                return;

            float p = healthModule.CurrentHealth / healthModule.MaxHealth;
            Vector3 s = new(1, 1, 1);
            s.x = p;
            fill.localScale = s;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (healthModule == null)
            {
                PlayerController player = FindAnyObjectByType<PlayerController>();

                if (player != null)
                    healthModule = player.GetComponent<HealthModule>();
            }
        }
#endif
    }
}
