using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerTrigger2D : MonoBehaviour
{
    public event Action<ulong> OnPlayerEnter;
    public event Action<ulong> OnPlayerExit;
    public event Action OnFirstPlayerEnter;
    public event Action OnLastPlayerExit;

    private int playerCount = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var playerNetObj = other.GetComponentInParent<NetworkObject>();
        if (playerNetObj != null && playerNetObj.IsPlayerObject)
        {
            ulong playerId = playerNetObj.OwnerClientId;

            bool wasEmpty = playerCount == 0;
            playerCount++;

            OnPlayerEnter?.Invoke(playerId);

            if (wasEmpty)
                OnFirstPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        var playerNetObj = other.GetComponentInParent<NetworkObject>();
        if (playerNetObj != null && playerNetObj.IsPlayerObject)
        {
            ulong playerId = playerNetObj.OwnerClientId;

            playerCount--;

            OnPlayerExit?.Invoke(playerId);

            if (playerCount == 0)
                OnLastPlayerExit?.Invoke();
        }
    }

    public bool HasPlayers() => playerCount > 0;
}
