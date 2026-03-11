using System.Linq;
using AncientDescent.Combat;
using AncientDescent.Player;
using UnityEngine;

namespace AncientDescent.Rooms
{
    public class SafeRoomBehaviour : RoomBehaviour
    {
        HealthModule playerHealth;
        [SerializeField] private float healPerSecond = 10f;

        private void Start()
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();

            if (player.TryGetComponent(out HealthModule ph))
                playerHealth = ph;
        }

        protected override void SubscribeToEvents()
        {

        }

        protected override void UnsubscribeFromEvents()
        {

        }

        private void FixedUpdate()
        {
            if (roomController.HasPlayer)
                HealPlayer();
        }

        private void HealPlayer()
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(healPerSecond * Time.fixedDeltaTime);
            }
        }
    }
}
