using UnityEngine;

namespace AncientDescent.CameraUtils
{
    public class CameraShakeSignal : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.5f;
        [SerializeField] private float frequency = 25f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private bool unscaledTime = false;

        private CameraShaker cameraShaker;

        private void Start()
        {
            cameraShaker = Camera.main?.GetComponent<CameraShaker>();
        }

        public void TriggerShake()
        {
            cameraShaker ??= Camera.main?.GetComponent<CameraShaker>();

            if (cameraShaker != null)
            {
                cameraShaker.Shake(frequency, amplitude, duration, unscaledTime);
            }
        }
    }
}