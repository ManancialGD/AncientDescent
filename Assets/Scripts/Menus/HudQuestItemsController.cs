using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HudQuestItemsController : MonoBehaviour
{
    [SerializeField]
    private HudQuestItem hudQuestItemPrefab;

    private PlayerInventory playerInventory;

    private void Awake()
    {
        playerInventory = FindAnyObjectByType<PlayerInventory>();
    }

    private void OnEnable()
    {
        if (playerInventory != null)
            playerInventory.QuestItemsUpdated += OnQuestItemUpdated;
    }

    private void OnDisable()
    {
        if (playerInventory != null)
            playerInventory.QuestItemsUpdated -= OnQuestItemUpdated;
    }

    private void OnQuestItemUpdated(List<QuestItemInstance> questItems)
    {
        Dictionary<QuestItemDefinition, int> defs = new();

        foreach (QuestItemInstance inst in questItems)
        {
            if (defs.ContainsKey(inst.Definition))
                defs[inst.Definition]++;
            else
                defs.Add(inst.Definition, 1);
        }

        UpdateItemsUI(defs);
    }

    private void UpdateItemsUI(Dictionary<QuestItemDefinition, int> items)
    {
        int index = 0;

        foreach (var kvp in items)
        {
            HudQuestItem hqi;

            if (index < transform.childCount)
            {
                hqi = transform.GetChild(index).GetComponent<HudQuestItem>();
            }
            else
            {
                hqi = Instantiate(hudQuestItemPrefab, transform);
            }

            hqi.Initialize(kvp.Key, kvp.Value);
            index++;
        }

        for (int i = transform.childCount - 1; i >= index; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
