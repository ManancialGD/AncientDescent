using System.Collections;
using System.Collections.Generic;
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

    [Header("Events")]
    [SerializeField] private UnityEvent OnCombatStarted;
    [SerializeField] private UnityEvent OnCombatCompleted;

    private bool isActive = false;
    private bool isCompleted = false;
    private readonly List<GameObject> spawnedEnemies = new();

    protected override void SubscribeToEvents()
    {
        if (activationTrigger != null)
        {
            activationTrigger.OnPlayerEnter += OnActivationTriggerEnter;
        }
    }

    protected override void UnsubscribeFromEvents()
    {
        if (activationTrigger != null)
        {
            activationTrigger.OnPlayerEnter -= OnActivationTriggerEnter;
        }
    }

    private void OnActivationTriggerEnter()
    {
        if (isActive || isCompleted) return;
        ActivateCombat();
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

    private void CloseDoors()
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
            spawnedEnemies.Add(enemy);

            if (enemy.TryGetComponent<HealthModule>(out var health))
            {
                health.Died += OnEnemyDied;
            }
        }
    }

    private void OnEnemyDied(HealthModule deadEnemy)
    {
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
    }

    public void ForceActivateCombat()
    {
        if (isActive || isCompleted) return;
        ActivateCombat();
    }
}
