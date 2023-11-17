using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCamera : MonoBehaviour
{
    private Camera playerCamera;
    InteractingHoodedRat interactingHoodedRat;
    public Transform lookTransform; // Assign the target transform to look at
    private Transform originalTransform; // To store the original position and rotation of the camera
    SettingVars settingVars;

    public float transitionSpeed = 1.0f;
    private bool isTransitioning = false;


    public float talkingDistance = 10;
    // Start is called before the first frame update
    void Start()
    {
        playerCamera = Camera.main;
        interactingHoodedRat = GetComponent<InteractingHoodedRat>();
        originalTransform = playerCamera.transform;
        settingVars = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();

    }

    // Update is called once per frame
    void Update()
    {
        if (interactingHoodedRat.isPlayerInZone && !isTransitioning)
        {
            StartCoroutine(MoveAndRotateCamera(lookTransform.position, lookTransform.rotation));
        }
        else if (!interactingHoodedRat.isPlayerInZone && isTransitioning)
        {
            StartCoroutine(MoveAndRotateCamera(originalTransform.position, originalTransform.rotation));
        }
    }

    // Call this method to move the camera to the dialogue position
    public void MoveToDialogue()
    {
        StartCoroutine(MoveAndRotateCamera(lookTransform.position - lookTransform.forward * talkingDistance, lookTransform.rotation));
    }

    // Call this method to move the camera back to the original position
    public void MoveBack()
    {
        StartCoroutine(MoveAndRotateCamera(originalTransform.position, originalTransform.rotation));
    }


    IEnumerator MoveAndRotateCamera(Vector3 targetPosition, Quaternion targetRotation)
    {
        isTransitioning = true;

        // Calculate a position offset that is 'talkingDistance' units away from the target position
        Vector3 directionToTarget = (targetPosition - playerCamera.transform.position).normalized;
        Vector3 adjustedTargetPosition = targetPosition - directionToTarget * talkingDistance;

        while (Vector3.Distance(playerCamera.transform.position, targetPosition) > 0.01f)
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, adjustedTargetPosition, transitionSpeed * Time.deltaTime);
            playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, targetRotation, transitionSpeed * Time.deltaTime);
            yield return null;
        }
        isTransitioning = false;
    }
}
