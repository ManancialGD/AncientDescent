using TMPro;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private int fuseNeeded = 3;
    [SerializeField] private int lockID;
    [SerializeField] private TextMeshProUGUI fuseCountText;

    private int currentFuses = 0;
    private bool unlocked = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (unlocked) return;

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
            if (fuseCountText != null)
                fuseCountText.text = $"{currentFuses}/{fuseNeeded}";
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
