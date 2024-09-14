using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [SerializeField] private Light flickerLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 100f;
    public float flickerSpeed = 0.01f; // Speed of the flicker effect
    private float flicker;
    private float defaultIntensity;
    private float randomIntensity;

    private void Awake()
    {
        if (!flickerLight)
        {
            if (TryGetComponent<Light>(out flickerLight))
            {
                defaultIntensity = flickerLight.intensity;
            }
        }
    }

    void Update()
    {
        flicker += Time.deltaTime;

        // Generate a random intensity within the specified range
        randomIntensity = Random.Range(minIntensity, maxIntensity);

        if (flicker >= flickerSpeed)
        {
            // Smoothly transition to the new intensity
            flickerLight.intensity = randomIntensity;
            flicker = 0f;
        }
    }
}
