using System.Linq;
using UnityEngine;

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
