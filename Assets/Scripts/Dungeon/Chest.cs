using Unity.Netcode;
using UnityEngine;

public class Chest : NetworkBehaviour
{
    private bool unlocked;
    private Animator animator;

    private bool alreadyOpened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Unlock()
    {
        if (!IsServer) return;
        unlocked = true;
        alreadyOpened = false;
    }

    public void Lock()
    {
        if (!IsServer) return;
        unlocked = false;
        alreadyOpened = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer || !unlocked || alreadyOpened) return;

        if (collision.TryGetComponent<PlayerKeys>(out var keys))
            keys.AddKeyServerRpc();

        if (animator != null)
            animator.SetTrigger("open");
        
        alreadyOpened = true;
    }
}
