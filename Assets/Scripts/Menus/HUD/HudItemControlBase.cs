using System.Collections.Generic;
using UnityEngine;

public abstract class HudItemControllerBase<TItem> : MonoBehaviour
{
    [SerializeField] protected HudItem itemPrefab;
    [SerializeField] protected Transform container;

    protected readonly List<HudItem> spawnedItems = new();

    // Derived classes must implement how to convert the raw list into display data
    protected abstract Dictionary<BaseItem, int> GetDisplayItems(List<TItem> items);

    protected void UpdateUI(List<TItem> items)
    {
        var displayItems = GetDisplayItems(items);

        int index = 0;
        foreach (var kvp in displayItems)
        {
            HudItem hudItem;

            // Reuse existing or create new
            if (index < spawnedItems.Count)
            {
                hudItem = spawnedItems[index];
            }
            else
            {
                hudItem = Instantiate(itemPrefab, container);
                spawnedItems.Add(hudItem);
            }

            hudItem.Initialize(kvp.Key, kvp.Value);
            index++;
        }

        // Destroy excess items
        for (int i = spawnedItems.Count - 1; i >= index; i--)
        {
            Destroy(spawnedItems[i].gameObject);
            spawnedItems.RemoveAt(i);
        }
    }
}
