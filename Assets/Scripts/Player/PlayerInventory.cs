using System.Collections.Generic;
using Unity.Netcode;

public class PlayerInventory : NetworkBehaviour
{
    private List<ItemInstance> items = new();
    private List<QuestItemInstance> questItems = new();
    private HealthModule health;

    private PlayerStats stats;

    public override void OnNetworkSpawn()
    {
        stats = GetComponent<PlayerStats>();
        health = GetComponent<HealthModule>();
    }

    public void AddItem(StatItemDefinition statItemDef)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var instance = new ItemInstance
        {
            Definition = statItemDef,
            StackCount = 1
        };

        items.Add(instance);

        ApplyItem(instance);
    }

    public void AddQuestItem(QuestItemDefinition def, int lockId)
    {
        if (!IsServer) return;

        questItems.Add(new QuestItemInstance
        {
            Definition = def,
            LockId = lockId
        });
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
        if (!IsServer) return 0;

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
