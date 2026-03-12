using System;
using AncientDescent.Interactables;
using AncientDescent.Player;
using Unity.Burst.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AncientDescent.Puzzles.SecondFloor
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Face : MonoBehaviour, IInteractable
    {
        [SerializeField] private Sprite smile;
        [SerializeField] private Sprite frown;
        [SerializeField] private Sprite surprise;
        [SerializeField] private Sprite sad;

        public Action<int, Sprite> FaceChanged;

        private SpriteRenderer spriteRenderer;

        private int i = -1;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(int index, Sprite[] order)
        {
            int r = Random.Range(0, order.Length);

            while (r == index)
            {
                r = Random.Range(0, order.Length);
            }

            spriteRenderer.sprite = order[r];
            i = index;
        }

        public bool CanInteract(PlayerController player)
        {
            return true;
        }

        public string GetInteractionPrompt(PlayerController player)
        {
            return "Rotate";
        }

        public void Interact(PlayerController player)
        {
            Sprite current = spriteRenderer.sprite;
            Sprite next = GetNextFace(current);
            spriteRenderer.sprite = next;
            FaceChanged?.Invoke(i, next);
        }

        private Sprite GetNextFace(Sprite current)
        {
            if (current == smile)
                return frown;
            if (current == frown)
                return surprise;
            if (current == surprise)
                return sad;
            return smile;
        }
    }
}
