using Unity.Netcode;
using UnityEngine;

public class Room : NetworkBehaviour, IRoom
{
    [field: SerializeField] public Transform[] EntryPoints { get; protected set; }
    [field: SerializeField] public Transform[] ExitPoints { get; protected set; }
    protected bool roomActive = false;

    public virtual void ActivateRoom()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        roomActive = true;
        gameObject.SetActive(true);
    }

    public virtual void DeactivateRoom()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        roomActive = false;
        gameObject.SetActive(false);
    }
}
