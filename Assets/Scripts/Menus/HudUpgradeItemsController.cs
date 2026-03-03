using System.Collections.Generic;
using UnityEngine;

public class HudUpgradeItemsController : HudItemControllerBase<ItemInstance>
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
            playerInventory.UpgradeItemsUpdated += OnUpgradeItemsUpdated;
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.UpgradeItemsUpdated -= OnUpgradeItemsUpdated;
    }

    private void OnUpgradeItemsUpdated(List<ItemInstance> upgradeItems)
    {
        UpdateUI(upgradeItems);
    }

    protected override Dictionary<Sprite, int> GetDisplayItems(List<ItemInstance> items)
    {
        var dict = new Dictionary<Sprite, int>();

        foreach (var inst in items)
        {
            if (inst.Definition == null || inst.Definition.Icon == null)
                continue;

            if (dict.ContainsKey(inst.Definition.Icon))
                dict[inst.Definition.Icon] += inst.StackCount;
            else
                dict[inst.Definition.Icon] = inst.StackCount;
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
