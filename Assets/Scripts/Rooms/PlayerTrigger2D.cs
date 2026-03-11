using System;
using AncientDescent.Player;
using UnityEngine;

namespace AncientDescent.Rooms
{
    public class PlayerTrigger2D : MonoBehaviour
    {
        public event Action OnPlayerEnter;
        public event Action OnPlayerExit;

        public bool HasPlayer { get; private set; } = false;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>())
            {
                OnPlayerEnter?.Invoke();
                HasPlayer = true;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.GetComponent<PlayerController>())
            {
                OnPlayerExit?.Invoke();
                HasPlayer = false;
            }
        }
    }
}
