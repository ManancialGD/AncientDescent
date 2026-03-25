using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AncientDescent.CameraUtils
{
    public class CameraShaker : MonoBehaviour
    {
        [Range(0f, 1f)] public float shakeIntensityMultiplier = 1f;

        private Vector3 basePosition;
        private readonly List<CameraShake> activeShakes = new();

        private void Awake()
        {
            basePosition = transform.localPosition;
        }

        private void LateUpdate()
        {
            Vector3 shakeOffset = Vector3.zero;

            // Remove finished shakes
            activeShakes.RemoveAll(s => s.Finished);

            if (activeShakes.Count == 0)
            {
                transform.localPosition = basePosition;
                return;
            }

            // Update elapsed time
            foreach (var shake in activeShakes)
            {
                float delta = shake.unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                shake.elapsed += delta;
            }

            // Pick the shake with the largest amplitude
            CameraShake strongest = null;
            foreach (var s in activeShakes)
            {
                if (strongest == null || s.amplitude > strongest.amplitude)
                    strongest = s;
            }

            if (strongest != null)
            {
                float t = strongest.unscaledTime ? Time.unscaledTime : Time.time;
                float x = (Mathf.PerlinNoise(t * strongest.frequency, 0f) * 2f - 1f) * strongest.amplitude;
                float y = (Mathf.PerlinNoise(0f, t * strongest.frequency) * 2f - 1f) * strongest.amplitude;
                shakeOffset = new Vector3(x, y, 0f) * shakeIntensityMultiplier;
            }

            // Apply additive offset to base position
            transform.localPosition = basePosition + shakeOffset;
        }

        /// <summary>
        /// Start a new camera shake.
        /// </summary>
        public void Shake(float frequency, float amplitude, float duration, bool unscaledTime = false)
        {
            if (amplitude <= 0f || duration <= 0f) return;

            activeShakes.Add(new CameraShake(frequency, amplitude, duration, unscaledTime));
        }

        /// <summary>
        /// Stops all shakes immediately.
        /// </summary>
        public void StopAllShakes()
        {
            activeShakes.Clear();
            transform.localPosition = basePosition;
        }

        private class CameraShake
        {
            public float frequency;
            public float amplitude;
            public float duration;
            public float elapsed;
            public bool unscaledTime;

            public CameraShake(float freq, float amp, float dur, bool unscaled)
            {
                frequency = freq;
                amplitude = amp;
                duration = dur;
                elapsed = 0f;
                unscaledTime = unscaled;
            }

            public float Progress => Mathf.Clamp01(elapsed / duration);
            public bool Finished => elapsed >= duration;
        }
    }
}