using Unity.Netcode;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;
    private bool canOpen = false;
    private bool isOpened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnCombatCompleted()
    {
        canOpen = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (isOpened || !canOpen) return;

        if (collision.gameObject.TryGetComponent(out PlayerKeys playerKeys))
        {
            playerKeys.AddKeyServerRpc();
            if (animator != null)
            {
                animator.SetTrigger("open");
            }
            isOpened = true;
            canOpen = false;
        }
    }
}
