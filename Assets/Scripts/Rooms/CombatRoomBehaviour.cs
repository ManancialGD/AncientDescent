using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CombatRoomBehaviour : RoomBehaviour
{
    [Header("Activation")]
    [SerializeField] private PlayerTrigger2D activationTrigger;

    [Header("Doors")]
    [SerializeField] private GameObject[] doorObjects;

    [Header("Spawner")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Settings")]
    [SerializeField] private bool resetOnPlayerExit = false;

    [Header("Events")]
    [SerializeField] private UnityEvent OnCombatStarted;
    [SerializeField] private UnityEvent OnCombatCompleted;

    private bool isActive = false;
    private bool isCompleted = false;
    private readonly List<GameObject> spawnedEnemies = new();
    private Coroutine resetCoroutine;

    protected override void SubscribeToEvents()
    {
        if (resetOnPlayerExit)
        {
            roomController.OnLastPlayerExitRoom += OnLastPlayerExitRoom;
        }

        if (activationTrigger != null)
        {
            activationTrigger.OnPlayerEnter += OnActivationTriggerEnter;
            activationTrigger.OnFirstPlayerEnter += OnFirstPlayerEnterActivation;
        }
    }

    protected override void UnsubscribeFromEvents()
    {
        roomController.OnLastPlayerExitRoom -= OnLastPlayerExitRoom;
        roomController.OnFirstPlayerEnterRoom -= OnFirstPlayerEnterRoom;

        if (activationTrigger != null)
        {
            activationTrigger.OnPlayerEnter -= OnActivationTriggerEnter;
            activationTrigger.OnFirstPlayerEnter -= OnFirstPlayerEnterActivation;
        }
    }

    private void OnActivationTriggerEnter(ulong playerId)
    {
        if (!NetworkManager.Singleton.IsServer || isActive || isCompleted) return;
        ActivateCombat();
    }

    private void OnFirstPlayerEnterActivation()
    {
        if (!NetworkManager.Singleton.IsServer || isActive || isCompleted) return;
        ActivateCombat();
    }

    private void OnFirstPlayerEnterRoom()
    {
        if (!NetworkManager.Singleton.IsServer || isActive || isCompleted) return;
        ActivateCombat();
    }

    private void OnLastPlayerExitRoom()
    {
        if (!NetworkManager.Singleton.IsServer || !isActive || isCompleted) return;

        if (resetCoroutine == null)
        {
            resetCoroutine = StartCoroutine(ResetCombatAfterDelay(2f));
        }
    }

    private IEnumerator ResetCombatAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (roomController.GetPlayersInRoom().Any())
        {
            ResetCombat();
        }

        resetCoroutine = null;
    }

    private void ActivateCombat()
    {
        isActive = true;

        CloseDoors();

        SpawnEnemies();

        if (activationTrigger != null)
        {
            activationTrigger.gameObject.SetActive(false);
        }

        OnCombatStarted?.Invoke();
    }

    private void ResetCombat()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                var netObj = enemy.GetComponent<NetworkObject>();
                if (netObj != null && netObj.IsSpawned)
                    netObj.Despawn();
            }
        }
        spawnedEnemies.Clear();

        OpenDoors();

        if (activationTrigger != null)
        {
            activationTrigger.gameObject.SetActive(true);
        }

        isActive = false;
    }

    private void CloseDoors()
    {
        foreach (GameObject door in doorObjects)
        {
            door.SetActive(true);
        }
        CloseDoorsClientRpc();
    }

    [ClientRpc]
    private void CloseDoorsClientRpc()
    {
        foreach (GameObject door in doorObjects)
        {
            door.SetActive(true);
        }
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            Transform spawnPoint = spawnPoints[i];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            var netObj = enemy.GetComponent<NetworkObject>();
            netObj.Spawn();

            spawnedEnemies.Add(enemy);

            if (enemy.TryGetComponent<HealthModule>(out var health))
            {
                health.Died += OnEnemyDied;
            }
        }
    }

    private void OnEnemyDied(HealthModule deadEnemy)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        deadEnemy.Died -= OnEnemyDied;
        spawnedEnemies.Remove(deadEnemy.gameObject);

        if (spawnedEnemies.Count == 0)
        {
            OnCombatCompleted?.Invoke();

            isCompleted = true;
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (GameObject door in doorObjects)
        {
            door.SetActive(false);
        }
        OpenDoorsClientRpc();
    }

    [ClientRpc]
    private void OpenDoorsClientRpc()
    {
        foreach (GameObject door in doorObjects)
        {
            door.SetActive(false);
        }
    }

    public void ForceActivateCombat()
    {
        if (!NetworkManager.Singleton.IsServer || isActive || isCompleted) return;
        ActivateCombat();
    }
}
