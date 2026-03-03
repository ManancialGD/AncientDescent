using TMPro;
using UnityEngine;

public class Engine : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private int fuseNeeded = 3;
    [SerializeField] private int lockID;
    private int currentFuses = 0;
    private bool unlocked = false;

    public bool CanInteract(PlayerController player)
    {
        return !unlocked;
    }

    public string GetInteractionPrompt(PlayerController player)
    {
        if (unlocked) return "";

        var inv = player.GetComponent<PlayerInventory>();
        bool hasFuses = inv != null && inv.HasQuestItem(lockID);

        if (hasFuses)
            return $"Insert Fuse ({currentFuses}/{fuseNeeded})";
        else
            return $"Needs Fuse ({currentFuses}/{fuseNeeded})";
    }

    public void Interact(PlayerController player)
    {
        if (unlocked) return;

        if (!player.TryGetComponent<PlayerInventory>(out var inventory)) return;

        int needed = fuseNeeded - currentFuses;
        int consumed = inventory.ConsumeQuestItems(lockID, needed);

        if (consumed > 0)
            currentFuses += consumed;


        if (currentFuses >= fuseNeeded)
            Unlock();
    }

    private void Unlock()
    {
        unlocked = true;
        if (doorObject != null)
            doorObject.SetActive(false);
    }
}
