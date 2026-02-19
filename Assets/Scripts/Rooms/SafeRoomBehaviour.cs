using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SafeRoomBehaviour : RoomBehaviour
{
    [SerializeField] private float healPerSecond = 10f;
    private bool isPlayerInside = false;
    protected override void SubscribeToEvents()
    {
        roomController.OnPlayerEnterRoom += OnPlayerEnter;
        roomController.OnPlayerExitRoom += OnPlayerExit;
    }
    protected override void UnsubscribeFromEvents()
    {
        roomController.OnPlayerEnterRoom -= OnPlayerEnter;
        roomController.OnPlayerExitRoom -= OnPlayerExit;
    }
    private void OnPlayerEnter(ulong playerId)
    {
        isPlayerInside = true;
    }
    private void OnPlayerExit(ulong playerId)
    {
        if (roomController.GetPlayersInRoom().Count() == 0)
            isPlayerInside = false;
    }
    private void FixedUpdate()
    {
        if (NetworkManager.Singleton == null) return;
        if (!NetworkManager.Singleton.IsServer) return;
        if (!isPlayerInside) return;

        HealAllPlayers();
    }

    private void HealAllPlayers()
    {
        foreach (ulong playerId in roomController.GetPlayersInRoom())
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerId, out var client))
            {
                var playerObj = client.PlayerObject; if (playerObj != null)
                {
                    if (playerObj.TryGetComponent<HealthModule>(out var health))
                        health.Heal(healPerSecond * Time.fixedDeltaTime);
                }
            }
        }
    }
}
