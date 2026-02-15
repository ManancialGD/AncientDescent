using Unity.Netcode;
using UnityEngine;

public class LookedDoor : MonoBehaviour
{
    [SerializeField] private GameObject doorObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        if (collision.gameObject.TryGetComponent(out PlayerKeys playerKeys))
        {
            if (playerKeys.ConsumeKey())
            {
                doorObject.SetActive(false);
            }
        }
    }
}