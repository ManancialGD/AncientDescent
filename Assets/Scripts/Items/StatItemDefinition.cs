using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Items/Stat Item")]
public class StatItemDefinition : BaseItem
{
    [SerializeField] private List<StatModifierData> modifiers;
    public IReadOnlyList<StatModifierData> Modifiers => modifiers;
}
