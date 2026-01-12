using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PortalRoom : Room
{
    public GameObject[] doorsToClose;

    [Header("Portal")]
    [SerializeField] private HealthModule portalHealthModule;

    [Header("Enemy Spawning")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] enemySpawnPoints;
    [SerializeField] private float enemySpawnRate = 2f;
    [SerializeField] private int maxAliveEnemies = 6;

    private Coroutine spawnRoutine;
    private bool isPortalDestroyed;

    private YieldInstruction waitForSpawnRate;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        waitForSpawnRate = new WaitForSeconds(enemySpawnRate);
        isPortalDestroyed = false;
        portalHealthModule.Died += HandlePortalDestruction;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (NetworkManager.Singleton == null)
            return;
        if (!NetworkManager.Singleton.IsServer)
            return;

        portalHealthModule.Died -= HandlePortalDestruction;
    }

    public override void ActivateRoom()
    {
        base.ActivateRoom();

        if (!NetworkManager.Singleton.IsServer || isPortalDestroyed)
            return;

        // Start spawning enemies when room activates
        spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());

        CloseDoors();
    }

    public override void DeactivateRoom()
    {
        base.DeactivateRoom();

        if (!NetworkManager.Singleton.IsServer)
            return;

        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        while (!isPortalDestroyed)
        {
            yield return waitForSpawnRate;

            if (isPortalDestroyed)
                yield break;

            if (GetAliveEnemyCount() >= maxAliveEnemies)
                continue;

            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint =
            enemySpawnPoints[Random.Range(0, enemySpawnPoints.Length)];

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        enemy.GetComponent<NetworkObject>().Spawn();
    }

    private int GetAliveEnemyCount()
    {
        return FindObjectsByType<MeleeEnemy>(FindObjectsSortMode.None).Length;
    }

    private void HandlePortalDestruction(HealthModule _)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        isPortalDestroyed = true;

        // Stop spawning
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }

        // Kill remaining enemies (optional but feels great)
        foreach (var enemy in FindObjectsByType<MeleeEnemy>(FindObjectsSortMode.None))
        {
            enemy.GetComponent<HealthModule>()?.Damage(
                portalHealthModule,
                9999f,
                0f
            );
        }

        OpenDoors();
    }

    private void CloseDoors()
    {
        foreach (Transform ExitPoint in ExitPoints)
        {
            ExitPoint.gameObject.SetActive(false);
        }

        foreach (var door in doorsToClose)
            door.SetActive(true);
    }

    private void OpenDoors()
    {
        foreach (Transform ExitPoint in ExitPoints)
        {
            ExitPoint.gameObject.SetActive(true);
        }

        foreach (var door in doorsToClose)
            door.SetActive(false);
    }

}
