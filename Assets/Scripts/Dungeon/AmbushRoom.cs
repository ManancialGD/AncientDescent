using Unity.Netcode;
using UnityEngine;

public class AmbushRoom : Room
{
    [Header("Ambush Settings")]
    [SerializeField] private Transform[] ambushSpawnPoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] doorsToClose;
    [SerializeField] private Chest rewardChest;

    private int aliveEnemies;
    private bool ambushTriggered;

    public override void ActivateRoom()
    {
        base.ActivateRoom();

        if (RoomCompleted) return;

        rewardChest.Lock();
        ambushTriggered = false;
    }

    public void TriggerAmbush()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (ambushTriggered) return;
        if (!roomActive) return;
        if (RoomCompleted) return;

        ambushTriggered = true;

        CloseDoors();
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        foreach (var spawn in ambushSpawnPoints)
        {
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var enemy = Instantiate(prefab, spawn.position, Quaternion.identity);
            enemy.GetComponent<NetworkObject>().Spawn();

            var health = enemy.GetComponent<HealthModule>();
            if (health != null)
            {
                health.Died += OnEnemyDied;
                aliveEnemies++;
            }
        }
    }

    private void OnEnemyDied(HealthModule enemy)
    {
        enemy.Died -= OnEnemyDied;
        aliveEnemies--;

        if (aliveEnemies <= 0)
            EndAmbush();
    }

    private void EndAmbush()
    {
        OpenDoors();
        rewardChest.Unlock();
        RoomCompleted = true;
    }

    private void CloseDoors()
    {
        foreach (var door in doorsToClose)
            door.SetActive(true);
    }

    private void OpenDoors()
    {
        foreach (var door in doorsToClose)
            door.SetActive(false);
    }
}
