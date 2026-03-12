using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class PulseLight2D : MonoBehaviour
{
    private Light2D light2D;

    [Header("Base Values")]
    public float baseRadius = 5f;
    public float baseIntensity = 1.2f;

    [Header("Flicker Amplitude")]
    public float radiusAmplitude = 0.5f;
    public float intensityAmplitude = 0.2f;

    [Header("Flicker Frequency")]
    public float radiusFrequency = 1.5f;
    public float intensityFrequency = 2f;

    [Header("Noise Settings")]
    public float noiseOffset = 0f;

    void Awake()
    {
        light2D = GetComponent<Light2D>();
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time;

        float radiusNoise = Mathf.PerlinNoise(noiseOffset, t * radiusFrequency);
        float intensityNoise = Mathf.PerlinNoise(noiseOffset + 100f, t * intensityFrequency);

        float radius = baseRadius + (radiusNoise - 0.5f) * 2f * radiusAmplitude;
        float intensity = baseIntensity + (intensityNoise - 0.5f) * 2f * intensityAmplitude;

        light2D.pointLightOuterRadius = radius;
        light2D.intensity = intensity;
    }
}