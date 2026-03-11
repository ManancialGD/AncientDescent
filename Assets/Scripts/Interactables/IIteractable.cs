using AncientDescent.Player;

namespace AncientDescent.Interactables
{
    public interface IInteractable
    {
        /// <summary>
        /// Checks if the object can be interacted with right now.
        /// </summary>
        bool CanInteract(PlayerController player);

        /// <summary>
        /// Returns the prompt text to display (e.g., "Open Chest", "Locked (Needs Key)").
        /// </summary>
        string GetInteractionPrompt(PlayerController player);

        /// <summary>
        /// Perform the interaction.
        /// </summary>
        void Interact(PlayerController player);
    }
}
