using UnityEngine;

public class EnemyStats : MonoBehaviour, IStatProvider
{
    [SerializeField] private float maxHealth = 100f;

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.MaxHealth => maxHealth,
            _ => 0f
        };
    }
}