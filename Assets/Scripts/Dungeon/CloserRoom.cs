using Unity.Netcode;
using UnityEngine;

public class CloserRoom : Room
{
    [Header("Room Settings")]
    public Transform[] enemySpawnPoints;
    public GameObject[] enemyPrefabs;
    public GameObject[] doorsToClose;

    private int aliveEnemies = 0;

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

    public override void ActivateRoom()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (roomActive)
            return;

        roomActive = true;
        gameObject.SetActive(true);
        CloseDoors();

        foreach (var spawn in enemySpawnPoints)
        {
            var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            var enemyObj = Instantiate(prefab, spawn.position, Quaternion.identity);
            enemyObj.GetComponent<NetworkObject>().Spawn();

            var health = enemyObj.GetComponent<HealthModule>();
            if (health != null)
            {
                health.Died += OnEnemyDied;
                aliveEnemies++;
            }
        }
    }

    public override void DeactivateRoom()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        roomActive = false;
        gameObject.SetActive(false);

        OpenDoors();
    }
    private void OnEnemyDied(HealthModule enemy)
    {
        aliveEnemies--;
        enemy.Died -= OnEnemyDied;

        if (aliveEnemies <= 0)
        {
            roomActive = false;
            OpenDoors();
        }
    }
}
