using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Entrance : MonoBehaviour
{
    [SerializeField]
    private string level2Scene = "";

    private bool canEnter = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out _))
        {
            if (canEnter)
                SceneManager.LoadScene(level2Scene);
        }
    }

    public void SetCanEnter(bool canEnter)
    {
        this.canEnter = canEnter;
    }
}
