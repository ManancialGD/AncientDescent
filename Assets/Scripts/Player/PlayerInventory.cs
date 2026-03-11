using System;
using System.Collections.Generic;
using AncientDescent.Combat;
using AncientDescent.Items;
using AncientDescent.Stats;
using UnityEngine;

namespace AncientDescent.Player
{    
    public class PlayerInventory : MonoBehaviour
    {
        public Action<List<QuestItemInstance>> QuestItemsUpdated;
        public Action<List<ItemInstance>> UpgradeItemsUpdated;
        private List<ItemInstance> items = new();
        private List<QuestItemInstance> questItems = new();
        private HealthModule health;
        private PlayerStats stats;
    
        private void Awake()
        {
            stats = GetComponent<PlayerStats>();
            health = GetComponent<HealthModule>();
        }
    
        public void AddItem(StatItemDefinition statItemDef)
        {
            var instance = new ItemInstance
            {
                Definition = statItemDef,
                StackCount = 1
            };
    
            items.Add(instance);
            ApplyItem(instance);
            UpgradeItemsUpdated?.Invoke(new List<ItemInstance>(items));
        }
    
        public void AddQuestItem(QuestItemDefinition def, int lockId)
        {
            questItems.Add(new QuestItemInstance
            {
                Definition = def,
                LockId = lockId
            });
            QuestItemsUpdated?.Invoke(questItems);
        }
    
        public bool HasQuestItem(int lockId)
        {
            for (int i = 0; i < questItems.Count; i++)
            {
                if (questItems[i].LockId == lockId)
                    return true;
            }
            return false;
        }
    
        public int ConsumeQuestItems(int lockId, int maxAmount)
        {
            int removed = 0;
    
            for (int i = questItems.Count - 1; i >= 0; i--)
            {
                if (questItems[i].LockId == lockId)
                {
                    questItems.RemoveAt(i);
                    removed++;
    
                    if (removed >= maxAmount)
                        break;
                }
            }
    
            QuestItemsUpdated?.Invoke(new List<QuestItemInstance>(questItems));
            return removed;
        }
    
        private void ApplyItem(ItemInstance item)
        {
            if (health != null)
                health.RecalculateMaxHealth();
    
            foreach (var modData in item.Definition.Modifiers)
            {
                stats.AddModifier(new StatModifier
                {
                    StatType = modData.statType,
                    Value = modData.value,
                    ModifierType = modData.modifierType,
                    Source = item
                });
            }
        }
    }
}
