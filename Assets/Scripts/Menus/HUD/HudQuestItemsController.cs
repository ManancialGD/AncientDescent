using System.Collections.Generic;
using UnityEngine;

public class HudQuestItemsController : HudItemControllerBase<QuestItemInstance>
{
    private PlayerInventory playerInventory;

    private void Awake()
    {
        //! FindObjects shouldn't be used in Awake. Only a fallback.
        playerInventory ??= FindAnyObjectByType<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.QuestItemsUpdated += OnQuestItemsUpdated;
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.QuestItemsUpdated -= OnQuestItemsUpdated;
    }

    private void OnQuestItemsUpdated(List<QuestItemInstance> questItems)
    {
        UpdateUI(questItems);
    }

    protected override Dictionary<BaseItem, int> GetDisplayItems(List<QuestItemInstance> items)
    {
        var dict = new Dictionary<BaseItem, int>();

        foreach (var inst in items)
        {
            if (inst.Definition == null || inst.Definition.Icon == null)
                continue;

            if (dict.ContainsKey(inst.Definition))
                dict[inst.Definition]++;
            else
                dict[inst.Definition] = 1;
        }

        return dict;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (playerInventory == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
                playerInventory = player.GetComponent<PlayerInventory>();
        }
    }
#endif  
}