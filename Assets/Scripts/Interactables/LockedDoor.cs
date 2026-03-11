using AncientDescent.Player;
using UnityEngine;

namespace AncientDescent.Interactables
{    
    public class LockedDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject doorObject;
        [SerializeField] private int lockID;
    
        public bool CanInteract(PlayerController player)
        {
            return doorObject != null && doorObject.activeSelf;
        }
    
        public string GetInteractionPrompt(PlayerController player)
        {
            if (doorObject == null || !doorObject.activeSelf) return "";
            
            var inv = player.GetComponent<PlayerInventory>();
            bool hasKey = inv != null && inv.HasQuestItem(lockID);
            
            return hasKey ? "Unlock Door" : "Locked (Needs Key)";
        }
    
        public void Interact(PlayerController player)
        {
            if (!CanInteract(player)) return;
    
            var inventory = player.GetComponent<PlayerInventory>();
            if (inventory == null) return;
    
            if (inventory.ConsumeQuestItems(lockID, 1) > 0)
            {
                doorObject.SetActive(false);
            }
        }
    }

}
