using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SafeRoomBehaviour : RoomBehaviour
{
    [SerializeField] private float healPerSecond = 10f;

    protected override void SubscribeToEvents() { }

    protected override void UnsubscribeFromEvents() { }

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;

        if (!roomController.GetPlayersInRoom().Any())
        {
            return;
        }

        HealAllPlayers();
    }

    private void HealAllPlayers()
    {
        foreach (ulong playerId in roomController.GetPlayersInRoom())
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
            {
                var playerObj = client.PlayerObject;
                if (playerObj != null)
                {
                    if (playerObj.TryGetComponent<HealthModule>(out var health))
                        health.Heal(healPerSecond * Time.fixedDeltaTime);

                }
            }
        }
    }
}
