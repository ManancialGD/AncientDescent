using Unity.Netcode;
using UnityEngine;

public class AmbushTrigger : NetworkBehaviour
{
    [SerializeField] private AmbushRoom room;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (!col.TryGetComponent<PlayerController>(out var player)) return;

        room.TriggerAmbush();
        gameObject.SetActive(false);
    }
}
