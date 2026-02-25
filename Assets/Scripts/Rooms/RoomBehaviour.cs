using UnityEngine;

[RequireComponent(typeof(RoomController))]
public abstract class RoomBehaviour : MonoBehaviour
{
    protected RoomController roomController;

    protected virtual void Awake()
    {
        roomController = GetComponent<RoomController>();
    }

    protected virtual void OnEnable()
    {
        if (roomController != null)
        {
            SubscribeToEvents();
        }
    }

    protected virtual void OnDisable()
    {
        if (roomController != null)
        {
            UnsubscribeFromEvents();
        }
    }

    protected abstract void SubscribeToEvents();
    protected abstract void UnsubscribeFromEvents();
}
