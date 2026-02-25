using UnityEngine;

public class PlayerHPBar : MonoBehaviour
{
    [SerializeField] private RectTransform fill;
    private HealthModule healthModule;

    private void Awake()
    {
        healthModule = GetComponentInParent<HealthModule>();
    }
    private void Start()
    {
        UpdateFill();
    }

    private void OnEnable()
    {
        if (healthModule == null)
            return;

        healthModule.Damaged += OnDamaged;
        healthModule.Healed += OnHealed;
    }

    private void OnDisable()
    {
        if (healthModule == null)
            return;

        healthModule.Damaged -= OnDamaged;
        healthModule.Healed -= OnHealed;
    }

    private void OnDamaged(DamageInfo damageInfo)
    {
        UpdateFill();
    }

    private void OnHealed(float _)
    {
        UpdateFill();
    }

    private void UpdateFill()
    {
        if (healthModule.MaxHealth <= 0)
            return;
        float p = healthModule.CurrentHealth / healthModule.MaxHealth;
        Vector3 s = new(1, 1, 1);
        s.x = p;
        fill.localScale = s;
    }
}
