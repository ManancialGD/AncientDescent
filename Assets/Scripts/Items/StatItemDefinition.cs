using UnityEngine;
using System.Collections.Generic;
using AncientDescent.Stats;

namespace AncientDescent.Items
{    
    [CreateAssetMenu(menuName = "Items/Stat Item")]
    public class StatItemDefinition : BaseItem
    {
        [SerializeField] private List<StatModifierData> modifiers;
        public IReadOnlyList<StatModifierData> Modifiers => modifiers;
    }

}
