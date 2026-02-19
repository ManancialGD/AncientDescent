using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Engine : NetworkBehaviour
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private int fuseNeeded = 3;
    [SerializeField] private int lockID;
    [SerializeField] private TextMeshProUGUI fuseCountText;

    private int currentFuses = 0;
    private bool unlocked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer || unlocked) return;

        if (collision.TryGetComponent(out PlayerInventory inventory))
        {
            TryAddFuses(inventory);
        }
    }

    private void TryAddFuses(PlayerInventory inventory)
    {
        if (unlocked) return;

        int needed = fuseNeeded - currentFuses;
        int consumed = inventory.ConsumeQuestItems(lockID, needed);

        if (consumed > 0)
        {
            currentFuses += consumed;
            fuseCountText.text = $"{currentFuses}/{fuseNeeded}";
            Debug.Log($"Inserted {consumed} fuse(s): {currentFuses}/{fuseNeeded}");
        }

        if (currentFuses >= fuseNeeded)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        unlocked = true;
        doorObject.SetActive(false);
    }
}
