using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{


    [Header("Base Settings")]
    public SettingVars settings;
    public Transform orientation;
    float xRot;
    float yRot;

    private float initialFOV;
    private Rigidbody playerPhybody;
    public AnimationCurve fovCurve;

    private void Awake()
    {
        noiseOffsetY = Random.Range(0f, 1f);
        initialFOV = Camera.main.fieldOfView;
        fovCurve = new AnimationCurve(new Keyframe[]
        {
            new Keyframe()
            {
                time=0,
                value = 0,
                outTangent = 0,
            },
            new Keyframe()
            {
                value = 1,
                time=1,
                inTangent = 2f
            }
        });
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerPhybody = Global.instance.sceneTree.Get("Player").GetComponent<Rigidbody>();
    }

    void Update()
    {
        float mouseX = settings.input.Gameplay.Look.ReadValue<Vector2>().x * Time.deltaTime * settings.CamSensitivityX;
        float mouseY = settings.input.Gameplay.Look.ReadValue<Vector2>().y * Time.deltaTime * settings.CamSensitivityY;

        yRot += mouseX;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);

        Camera.main.fieldOfView = initialFOV + (10f * fovCurve.Evaluate(Mathf.Clamp(playerPhybody.velocity.magnitude, 0.01f, 30f) / 30));
    }

    [Header("Screenshake Shake Options")]
    // Duration for how long the shake should last
    public float duration;

    // Magnitude of positional shake
    public float magnitude;

    // Magnitude of rotational shake, in degrees
    public float rotationalMagnitude;

    // Time it takes for the camera to smoothly return to its original position/rotation after shaking
    public float returnDuration;

    // Offset for Perlin noise to ensure randomness across axes
    private float noiseOffsetY;

    public void shake()
    {
        float PerlinNoiseOffset(float time, float magnitude)
        {
            // Adjusts the Perlin noise value (0 to 1) to range from -magnitude to +magnitude
            return (Mathf.PerlinNoise(time, 0.0f) - 0.5f) * 2.0f * magnitude;
        }

        IEnumerator shakeRoutine()
        {
            // Keep track of elapsed time since shaking started
            float elapsed = 0.0f;



            // Store the camera's original position and rotation
            Vector3 originalPosition = transform.localPosition;
            Quaternion originalRotation = transform.localRotation;
            //float originalFOV = Camera.main.fieldOfView; // Make sure your camera is tagged as "MainCamera"


            // Perform the shake while elapsed time is less than the set duration
            while (elapsed < duration)
            {
                // Calculate noise-based positional offsets for each axis using elapsed time and the magnitude
                // Different offsets ensure unique shake patterns for each axis
                float x = PerlinNoiseOffset(elapsed, magnitude);
                float y = PerlinNoiseOffset(elapsed + noiseOffsetY, magnitude);

                float fovOffset = PerlinNoiseOffset(elapsed + 2 * noiseOffsetY, magnitude * 10); // You might need to adjust the multiplier
                //Camera.main.fieldOfView = originalFOV + fovOffset;
                transform.localPosition = new Vector3(x, y, originalPosition.z);



                // Increment elapsed time
                elapsed += Time.deltaTime;
                yield return null;
            }

            // After shaking, smoothly return the camera to its original position and rotation over 'returnDuration' time
            float returnElapsed = 0.0f;
            Vector3 startingPosition = transform.localPosition;
            //float startingFOV = Camera.main.fieldOfView;

            while (returnElapsed < returnDuration)
            {
                returnElapsed += Time.deltaTime;
                float normalizedTime = returnElapsed / returnDuration;

                // Interpolate position and rotation based on elapsed time to give a smooth return
                transform.localPosition = Vector3.Lerp(startingPosition, originalPosition, normalizedTime);

                // Smoothly return FOV to its original value
                //Camera.main.fieldOfView = Mathf.Lerp(startingFOV, originalFOV, normalizedTime);

                yield return null;
            }
        }

        StartCoroutine(shakeRoutine());
    }
}
