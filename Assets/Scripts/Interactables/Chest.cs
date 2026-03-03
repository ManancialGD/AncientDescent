using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private ChestLootEntry[] itemsToGive;
    private Animator animator;
    private bool canOpen = false;
    private bool isOpened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnCombatCompleted()
    {
        canOpen = true;
    }

    public bool CanInteract(PlayerController player)
    {
        return !isOpened && canOpen;
    }

    public string GetInteractionPrompt(PlayerController player)
    {
        if (isOpened) return "";
        if (!canOpen) return "Locked";
        return "Open Chest";
    }

    public void Interact(PlayerController player)
    {
        if (!CanInteract(player)) return;

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        foreach (ChestLootEntry item in itemsToGive)
        {
            if (item.itemDefinition is QuestItemDefinition questItemDef)
            {
                inventory.AddQuestItem(questItemDef, item.lockId);
            }
            else if (item.itemDefinition is StatItemDefinition statItemDef)
            {
                inventory.AddItem(statItemDef);
            }
        }

        if (animator != null)
        {
            animator.SetTrigger("open");
        }
        isOpened = true;
        canOpen = false;
    }
}
