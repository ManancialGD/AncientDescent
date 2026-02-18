using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
        }
    }

    private void OnSceneLoadCompleted(
        string sceneName,
        LoadSceneMode loadSceneMode,
        List<ulong> clientsCompleted,
        List<ulong> clientsTimedOut)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        int i = 0;
        foreach (var clientId in clientsCompleted)
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
                continue;

            var player = client.PlayerObject;
            if (player == null)
                continue;

            player.transform.position =
                spawnPoints[i % spawnPoints.Length].position;
            i++;
        }
    }

    public void RespawnPlayersAtStart()
    {
        if (!NetworkManager.Singleton.IsServer) return;

        int i = 0;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                client.PlayerObject.transform.position =
                    spawnPoints[i % spawnPoints.Length].position;
                i++;
            }
        }
    }
}
