using AncientDescent.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AncientDescent.Rooms.Puzzles
{
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
}
