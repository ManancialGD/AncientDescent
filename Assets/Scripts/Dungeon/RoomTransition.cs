using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class RoomTransition : NetworkBehaviour
{

    private static readonly float transitionDelay = 0.55f;
    [SerializeField] private Room from;
    [SerializeField] private Room to;
    [SerializeField] private int exitPoint;
    [SerializeField] private TransitionDirection direction;
    private TransitionUI transitionUI;
    private readonly HashSet<NetworkObject> playersInTrigger = new();
    private static bool transitionInProgress;

    private void Start()
    {
        transitionUI = FindAnyObjectByType<TransitionUI>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) return;
        if (transitionInProgress) return;

        if (!col.TryGetComponent(out NetworkObject netObj)) return;
        if (!netObj.TryGetComponent<PlayerController>(out _)) return;

        playersInTrigger.Add(netObj);

        if (playersInTrigger.Count == NetworkManager.Singleton.ConnectedClients.Count)
        {
            transitionInProgress = true;
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        playersInTrigger.Clear();

        // Lock player controls
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent(out PlayerController pc))
            {
                pc.SetPlayerState(PlayerControlState.Transition);
            }
        }

        StartTransitionClientRpc(direction);

        yield return new WaitForSeconds(transitionDelay);

        // Teleport players
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.transform.position =
                to.ExitPoints[exitPoint].position;
        }

        from.DeactivateRoom();
        to.ActivateRoom();

        EndTransitionClientRpc();

        // Restore gameplay
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject.TryGetComponent(out PlayerController pc))
            {
                pc.SetPlayerState(PlayerControlState.Gameplay);
            }
        }

        transitionInProgress = false;
    }


    [ClientRpc]
    private void StartTransitionClientRpc(TransitionDirection dir)
    {
        if (transitionUI == null)
            transitionUI = FindAnyObjectByType<TransitionUI>();

        if (transitionUI == null)
            return;

        transitionUI.StartCoroutine(
                transitionUI.Close(dir)
            );
    }

    [ClientRpc]
    private void EndTransitionClientRpc()
    {
        if (transitionUI == null)
            transitionUI = FindAnyObjectByType<TransitionUI>();

        if (transitionUI == null)
            return;

        transitionUI.StartCoroutine(
                transitionUI.Open()
            );
    }
}
