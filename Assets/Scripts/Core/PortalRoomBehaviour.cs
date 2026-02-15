using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PortalRoomBehaviour : RoomBehaviour
{
    [Header("Portal")]
    [SerializeField] private HealthModule portalHealthModule;
    [Header("Doors")]
    [SerializeField] private GameObject[] doorObjects;

    [Header("Spawner")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int maxAliveEnemies = 5;

    [Header("Events")]
    [SerializeField] private UnityEvent OnCombatStarted;
    [SerializeField] private UnityEvent OnPortalDestroyed;

    private Coroutine spawnRoutine;

    private YieldInstruction waitForSpawnRate;

    private bool isActive = false;
    private bool isCompleted = false;
    private bool isPortalDestroyed = false;
    private readonly List<GameObject> spawnedEnemies = new();

    protected override void Awake()
    {
        base.Awake();
        waitForSpawnRate = new WaitForSeconds(spawnRate);
    }

    protected override void SubscribeToEvents()
    {
        roomController.OnFirstPlayerEnterRoom += OnFirstPlayerEnterRoom;
        if (portalHealthModule != null)
        {
            portalHealthModule.Died += HandlePortalDestruction;
        }
    }

    protected override void UnsubscribeFromEvents()
    {
        roomController.OnFirstPlayerEnterRoom -= OnFirstPlayerEnterRoom;
        if (portalHealthModule != null)
        {
            portalHealthModule.Died -= HandlePortalDestruction;
        }
    }

    private void OnFirstPlayerEnterRoom()
    {
        if (!NetworkManager.Singleton.IsServer || isActive || isCompleted) return;
        ActivateCombat();
    }

    private void ActivateCombat()
    {
        isActive = true;

        CloseDoors();

        spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());

        OnCombatStarted?.Invoke();
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

    private void OnEnemyDied(HealthModule deadEnemy)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        deadEnemy.Died -= OnEnemyDied;
        spawnedEnemies.Remove(deadEnemy.gameObject);
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

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (!isPortalDestroyed)
        {
            yield return waitForSpawnRate;

            if (isPortalDestroyed)
                yield break;

            if (spawnedEnemies.Count >= maxAliveEnemies)
                continue;

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        enemy.GetComponent<NetworkObject>().Spawn();

        spawnedEnemies.Add(enemy);

        if (enemy.TryGetComponent<HealthModule>(out var health))
        {
            health.Died += OnEnemyDied;
        }
    }

    private void HandlePortalDestruction(HealthModule _)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        isPortalDestroyed = true;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        foreach (var enemy in spawnedEnemies.ToList())
        {
            enemy.GetComponent<HealthModule>()?.Damage(
                null,
                9999f,
                0f
            );
        }

        OpenDoors();
    }
}