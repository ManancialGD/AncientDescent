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

    private void Start()
    {
        transitionUI = FindAnyObjectByType<TransitionUI>();
    }

    private readonly HashSet<NetworkObject> playersInTrigger = new();

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) return;

        if (!col.TryGetComponent<NetworkObject>(out var netObj)) return;
        if (!netObj.TryGetComponent<PlayerController>(out _)) return;

        playersInTrigger.Add(netObj);

        if (playersInTrigger.Count == NetworkManager.Singleton.ConnectedClients.Count)
        {
            StartCoroutine(TransitionRoutine());
        }
    }

    private IEnumerator TransitionRoutine()
    {
        playersInTrigger.Clear();

        // Close screen
        StartTransitionClientRpc(direction);

        yield return new WaitForSeconds(transitionDelay);

        // Teleport players
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            client.PlayerObject.transform.position =
                to.ExitPoints[exitPoint].position;
        }

        // Room swap
        from.DeactivateRoom();
        to.ActivateRoom();

        // Reveal
        EndTransitionClientRpc();
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
