using Unity.Netcode;
using UnityEngine;

public class LockedDoor : NetworkBehaviour
{
    [field: SerializeField] public GameObject DoorVisual { get; private set; }
    private bool locked = true;

    private void Start()
    {
        Lock();
    }

    public void Unlock()
    {
        if (!IsServer) return;
        locked = false;
        DoorVisual.SetActive(false);
    }

    public void Lock()
    {
        if (!IsServer) return;
        locked = true;
        DoorVisual.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer || !locked) return;

        if (collision.TryGetComponent<PlayerKeys>(out var keys))
        {
            if (keys.ConsumeKey())
            {
                Unlock();
            }
        }
    }
}
