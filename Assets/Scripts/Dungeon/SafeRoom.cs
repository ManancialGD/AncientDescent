using Unity.Netcode;
using UnityEngine;

public class SafeRoom : Room
{
    [field: SerializeField] public Transform[] SpawnPoints { get; private set; }
    [SerializeField] private float healAmountPerSecond = 15f;

    private void FixedUpdate()
    {
        if (NetworkManager.Singleton == null)
            return;

        if (!NetworkManager.Singleton.IsServer)
            return;

        if (!roomActive)
            return;

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (player.PlayerObject != null &&
                player.PlayerObject.TryGetComponent(out HealthModule health))
            {
                health.Heal(healAmountPerSecond * Time.fixedDeltaTime);
            }
        }
    }
}
