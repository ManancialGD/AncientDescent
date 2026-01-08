using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameManager : NetworkBehaviour
{
    [SerializeField] private SafeRoom startRoom;
    [SerializeField] private Room[] otherRooms;

    private void OnEnable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
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
                startRoom.SpawnPoints[i % startRoom.ExitPoints.Length].position;
            i++;
        }

        startRoom.ActivateRoom();
        foreach (var room in otherRooms)
        {
            room.DeactivateRoom();
        }
    }


}