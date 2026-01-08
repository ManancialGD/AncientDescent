using UnityEngine;

public interface IRoom
{
    public Transform[] EntryPoints { get; }
    public Transform[] ExitPoints { get; }
    void ActivateRoom();
    void DeactivateRoom();
}
