using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;
    [SerializeField] private int lockID;

    public void TryOpen(PlayerInventory inventory)
    {
        if (inventory.ConsumeQuestItems(lockID, 1) > 0)
        {
            doorObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out PlayerInventory inventory))
        {
            TryOpen(inventory);
        }
    }
}
