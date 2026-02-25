using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room Boundary")]
    [SerializeField] private PlayerTrigger2D boundaryTrigger;

    public event Action OnPlayerEnterRoom;
    public event Action OnPlayerExitRoom;

    public bool HasPlayer => boundaryTrigger != null ? boundaryTrigger.HasPlayer : false;

    private void Awake()
    {
        if (boundaryTrigger != null)
        {
            boundaryTrigger.OnPlayerEnter += OnBoundaryPlayerEnter;
            boundaryTrigger.OnPlayerExit += OnBoundaryPlayerExit;
        }
    }

    private void OnDestroy()
    {
        if (boundaryTrigger != null)
        {
            boundaryTrigger.OnPlayerEnter -= OnBoundaryPlayerEnter;
            boundaryTrigger.OnPlayerExit -= OnBoundaryPlayerExit;
        }
    }

    private void OnBoundaryPlayerEnter()
    {
        OnPlayerEnterRoom?.Invoke();
    }

    private void OnBoundaryPlayerExit()
    {
        OnPlayerExitRoom?.Invoke();
    }
}
