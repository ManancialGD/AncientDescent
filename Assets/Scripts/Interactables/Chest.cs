using Unity.Netcode;
using UnityEngine;

public class Chest : MonoBehaviour
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (isOpened || !canOpen) return;

        if (collision.gameObject.TryGetComponent(out PlayerInventory inventory))
        {
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
}
