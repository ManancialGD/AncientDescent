using AncientDescent.Player;
using TMPro;
using UnityEngine;

namespace AncientDescent.Interactables
{
    [RequireComponent(typeof(Canvas))]
    public class InteractablePrompt : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI promptText;
        private Canvas canvas;
        private IInteractable interactable;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            canvas.enabled = false; // Start hidden

            interactable = GetComponentInParent<IInteractable>();

            if (interactable == null)
                Debug.LogError("InteractablePrompt must be a child of an IInteractable object", this);

            if (promptText == null)
                Debug.LogError("Prompt Text is not assigned on InteractablePrompt", this);
        }

        /// <summary>
        /// Show or hide the prompt, updating the text if visible.
        /// </summary>
        public void SetVisible(bool visible, PlayerController player)
        {
            if (visible)
            {
                if (interactable != null && promptText != null)
                    promptText.text = interactable.GetInteractionPrompt(player);

                canvas.enabled = true;
            }
            else
            {
                canvas.enabled = false;
            }
        }

        /// <summary>
        /// Force refresh the prompt text without changing visibility.
        /// </summary>
        public void RefreshText(PlayerController player)
        {
            if (canvas.enabled && interactable != null && promptText != null)
            {
                string prompt = interactable.GetInteractionPrompt(player);

                if (string.IsNullOrEmpty(prompt))
                    canvas.enabled = false;
                else
                    promptText.text = interactable.GetInteractionPrompt(player);
            }
        }
    }
}
