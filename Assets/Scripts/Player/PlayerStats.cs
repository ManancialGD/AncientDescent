using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IStatProvider
{
    [SerializeField] private float startMaxHealth = 100f;
    [SerializeField] private float startMoveSpeed = 80f;
    [SerializeField] private float startAcceleration = 15f;
    [SerializeField] private float startFriction = 8f;
    [SerializeField] private float startShootRate = 0.2f;

    private readonly Dictionary<StatType, float> baseStats = new();
    private readonly Dictionary<StatType, List<StatModifier>> modifiers = new();

    private void Awake()
    {
        baseStats[StatType.MaxHealth] = startMaxHealth;
        baseStats[StatType.MoveSpeed] = startMoveSpeed;
        baseStats[StatType.Acceleration] = startAcceleration;
        baseStats[StatType.Friction] = startFriction;
        baseStats[StatType.ShootRate] = startShootRate;
    }

    public float GetStat(StatType type)
    {
        if (!baseStats.TryGetValue(type, out float baseValue))
            return 0f;

        if (!modifiers.TryGetValue(type, out var modList) || modList.Count == 0)
            return baseValue;

        float value = baseValue;

        foreach (var mod in modList)
        {
            switch (mod.ModifierType)
            {
                case ModifierType.Set:
                    value = mod.Value;
                    break;
                case ModifierType.Add:
                    value += mod.Value;
                    break;
                case ModifierType.Remove:
                    value -= mod.Value;
                    break;
                case ModifierType.Multiply:
                    value *= mod.Value;
                    break;
                case ModifierType.Divide:
                    value /= mod.Value;
                    break;
            }
        }

        return value;
    }

    public void AddModifier(StatModifier modifier)
    {
        if (!modifiers.ContainsKey(modifier.StatType))
            modifiers[modifier.StatType] = new();

        modifiers[modifier.StatType].Add(modifier);
    }

    public void RemoveSource(object source)
    {
        foreach (var kvp in modifiers)
        {
            kvp.Value.RemoveAll(m => m.Source == source);
        }
    }
}
