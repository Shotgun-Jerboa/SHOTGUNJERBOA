using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilCameraShake : MonoBehaviour
{
    // Duration for how long the shake should last
    public float duration = 0.5f;

    // Magnitude of positional shake
    public float magnitude = 0.1f;

    // Magnitude of rotational shake, in degrees
    public float rotationalMagnitude = 5.0f;

    // Time it takes for the camera to smoothly return to its original position/rotation after shaking
    public float returnDuration = 0.2f;

    // Offset for Perlin noise to ensure randomness across axes
    private float noiseOffsetY;

    // Initialize the noiseOffsetY with a random value between 0 and 1 at the start
    private void Awake()
    {
        noiseOffsetY = Random.Range(0f, 1f);
    }

    // Call this function to start the camera shake
    public void ShakeCamera()
    {
        StartCoroutine(Shake());
    }

    // Coroutine to handle the shaking effect
    System.Collections.IEnumerator Shake()
    {
        // Keep track of elapsed time since shaking started
        float elapsed = 0.0f;



        // Store the camera's original position and rotation
        Vector3 originalPosition = transform.localPosition;
        Quaternion originalRotation = transform.localRotation;
        float originalFOV = Camera.main.fieldOfView; // Make sure your camera is tagged as "MainCamera"


        // Perform the shake while elapsed time is less than the set duration
        while (elapsed < duration)
        {
            // Calculate noise-based positional offsets for each axis using elapsed time and the magnitude
            // Different offsets ensure unique shake patterns for each axis
            float x = PerlinNoiseOffset(elapsed, magnitude);
            float y = PerlinNoiseOffset(elapsed + noiseOffsetY, magnitude);

            float fovOffset = PerlinNoiseOffset(elapsed + 2 * noiseOffsetY, magnitude * 10); // You might need to adjust the multiplier
            Camera.main.fieldOfView = originalFOV + fovOffset;
            transform.localPosition = new Vector3(x, y, originalPosition.z);

           

            // Increment elapsed time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // After shaking, smoothly return the camera to its original position and rotation over 'returnDuration' time
        float returnElapsed = 0.0f;
        Vector3 startingPosition = transform.localPosition;
        float startingFOV = Camera.main.fieldOfView;

        while (returnElapsed < returnDuration)
        {
            returnElapsed += Time.deltaTime;
            float normalizedTime = returnElapsed / returnDuration;

            // Interpolate position and rotation based on elapsed time to give a smooth return
            transform.localPosition = Vector3.Lerp(startingPosition, originalPosition, normalizedTime);

            // Smoothly return FOV to its original value
            Camera.main.fieldOfView = Mathf.Lerp(startingFOV, originalFOV, normalizedTime);

            yield return null;
        }
    }

    // Function to compute an offset value using Perlin noise
    float PerlinNoiseOffset(float time, float magnitude)
    {
        // Adjusts the Perlin noise value (0 to 1) to range from -magnitude to +magnitude
        return (Mathf.PerlinNoise(time, 0.0f) - 0.5f) * 2.0f * magnitude;
    }
}
