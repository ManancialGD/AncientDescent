using AncientDescent.Input;
using AncientDescent.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AncientDescent.Player
{    
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableLayers = -1;
        [SerializeField] private Transform detectionOrigin;
    
        private PlayerController player;
        private PlayerControls controls;
        private IInteractable currentInteractable;
        private InteractablePrompt currentPrompt;
    
        private void Awake()
        {
            player = GetComponent<PlayerController>();
    
            if (detectionOrigin == null) 
                detectionOrigin = transform;
        }
    
        private void Update()
        {
            FindClosestInteractable();
        }
    
        private void FindClosestInteractable()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(detectionOrigin.position, interactionRange, interactableLayers);
            IInteractable closest = null;
            float closestDistSqr = Mathf.Infinity;
            Transform closestTransform = null;
    
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<IInteractable>(out var interactable))
                {
                    float distSqr = (hit.transform.position - detectionOrigin.position).sqrMagnitude;
                    if (distSqr < closestDistSqr)
                    {
                        closestDistSqr = distSqr;
                        closest = interactable;
                        closestTransform = hit.transform;
                    }
                }
            }
    
            if (closest != currentInteractable)
            {
                if (currentPrompt != null)
                    currentPrompt.SetVisible(false, player);
    
                currentInteractable = closest;
    
                if (currentInteractable != null && closestTransform != null)
                {
                    currentPrompt = closestTransform.GetComponent<InteractablePrompt>();
                    if (currentPrompt == null)
                        currentPrompt = closestTransform.GetComponentInChildren<InteractablePrompt>();
    
                    if (currentPrompt != null)
                        currentPrompt.SetVisible(true, player);
                    else
                        Debug.LogWarning($"Interactable {closestTransform.name} has no InteractablePrompt component");
                }
                else
                {
                    currentPrompt = null;
                }
            }
            else if (currentInteractable != null && currentPrompt != null)
            {
                currentPrompt.RefreshText(player);
            }
        }
    
        public void OnInteractPerformed(InputAction.CallbackContext ctx)
        {
            if (currentInteractable != null && currentInteractable.CanInteract(player))
            {
                currentInteractable.Interact(player);
            }
        }
    
    #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(detectionOrigin != null ? detectionOrigin.position : transform.position, interactionRange);
        }
    #endif
    }
}
