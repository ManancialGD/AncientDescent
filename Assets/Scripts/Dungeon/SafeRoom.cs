using Unity.Netcode;
using UnityEngine;

public class SafeRoom : Room
{
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }
    [SerializeField] private float healAmountPerSecond = 15f;

    private void FixedUpdate()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (!roomActive)
            return;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (player.PlayerObject.TryGetComponent<HealthModule>(out var health))
            {
                health.Heal(healAmountPerSecond * Time.fixedDeltaTime);
            }
        }
    }
}
