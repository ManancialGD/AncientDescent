using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoomController : NetworkBehaviour
{
    [Header("Room Boundary")]
    [SerializeField] private PlayerTrigger2D boundaryTrigger;
    public event Action<ulong> OnPlayerEnterRoom;
    public event Action<ulong> OnPlayerExitRoom;
    public event Action OnFirstPlayerEnterRoom;
    public event Action OnLastPlayerExitRoom;

    private readonly NetworkList<ulong> playersInRoom = new();

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            playersInRoom.OnListChanged += OnPlayersListChanged;

            if (boundaryTrigger != null)
            {
                boundaryTrigger.OnPlayerEnter += OnBoundaryPlayerEnter;
                boundaryTrigger.OnPlayerExit += OnBoundaryPlayerExit;
                boundaryTrigger.OnFirstPlayerEnter += OnBoundaryFirstPlayerEnter;
                boundaryTrigger.OnLastPlayerExit += OnBoundaryLastPlayerExit;
            }
        }

    }

    public override void OnNetworkDespawn()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            playersInRoom.OnListChanged -= OnPlayersListChanged;

            if (boundaryTrigger != null)
            {
                boundaryTrigger.OnPlayerEnter -= OnBoundaryPlayerEnter;
                boundaryTrigger.OnPlayerExit -= OnBoundaryPlayerExit;
                boundaryTrigger.OnFirstPlayerEnter -= OnBoundaryFirstPlayerEnter;
                boundaryTrigger.OnLastPlayerExit -= OnBoundaryLastPlayerExit;
            }
        }

    }

    private void OnBoundaryPlayerEnter(ulong playerId)
    {
        if (!playersInRoom.Contains(playerId))
            playersInRoom.Add(playerId);

        OnPlayerEnterRoom?.Invoke(playerId);

    }

    private void OnBoundaryPlayerExit(ulong playerId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (playersInRoom.Contains(playerId))
                playersInRoom.Remove(playerId);
        }

        OnPlayerExitRoom?.Invoke(playerId);
    }

    private void OnBoundaryFirstPlayerEnter()
    {
        OnFirstPlayerEnterRoom?.Invoke();
    }

    private void OnBoundaryLastPlayerExit()
    {
        OnLastPlayerExitRoom?.Invoke();
    }

    private void OnPlayersListChanged(NetworkListEvent<ulong> changeEvent)
    {
        
    }

    public IEnumerable<ulong> GetPlayersInRoom()
    {
        foreach (var id in playersInRoom)
            yield return id;
    }

}
