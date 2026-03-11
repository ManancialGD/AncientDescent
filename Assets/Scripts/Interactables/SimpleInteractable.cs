using AncientDescent.Player;
using UnityEngine;
using UnityEngine.Events;

namespace AncientDescent.Interactables
{    
    public class SimpleInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private string canInteractPrompt = "Interact";
        [SerializeField]
        private string cannotInteractPrompt = "";
    
        [SerializeField]
        private bool startDeactivated = false;
    
        [SerializeField]
        private UnityEvent OnInteract;
    
        private bool active;
    
        private void Start()
        {
            active = !startDeactivated;
        }
    
        public bool CanInteract(PlayerController player)
        {
            return active;
        }
    
        public string GetInteractionPrompt(PlayerController player)
        {
            return CanInteract(player) ? canInteractPrompt : cannotInteractPrompt;
        }
    
        public void Interact(PlayerController player)
        {
            if (CanInteract(player))
                OnInteract.Invoke();
        }
    
        public void SetActive(bool isActive)
        {
            active = isActive;
        }
    }

}
