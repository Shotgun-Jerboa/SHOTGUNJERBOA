using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public LayerMask whatIsPlayer, obstructionMask;

    [Header("Field of View Stat")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    public float maxPurseDistance; //If the player move pass this then move to back to patroll

    public bool playerInSightRange;
    public bool hasSpottedPlayer; // New variable to maintain whether the player has been spotted


    //This is just for FieldOfView Editor, which is to visualize FOV
    [HideInInspector] public GameObject playerRef;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FOVRoutine());
        playerRef = Global.instance.sceneTree.Get("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator FOVRoutine()
    {
        float delay = 0.1f;

        WaitForSeconds wait = new WaitForSeconds(delay);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        //Check colliders in FOV
        Collider[] rangeCheck = Physics.OverlapSphere(transform.position, radius, whatIsPlayer);

        //We find our colliders with target layerMask
        if (rangeCheck.Length != 0)
        {
            //There's only 1 player so we get the first index of the target list
            Transform target = rangeCheck[0].transform;

            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float verticalAngle = Vector3.Angle(transform.up, directionToTarget); // Vertical angle check

                if (verticalAngle < angle / 2) // Vertical angle check
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);


                    //If there's no collider infront of the ray cast with layer obstruction Mask, then we can see player 
                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        playerInSightRange = true;
                        hasSpottedPlayer = true; // Set the variable true when the player is spotted
                    }
                    else
                    {
                        playerInSightRange = false;
                    }
                    // Additional condition to reset hasSpottedPlayer
                    if (hasSpottedPlayer)
                    {
                        if (Vector3.Distance(transform.position, playerRef.transform.position) > maxPurseDistance)
                        {
                            hasSpottedPlayer = false; // Reset the variable if the player is too far away
                        }
                    }
                }
                else
                {
                    playerInSightRange = false;
                }
            }
            else if (playerInSightRange)
            {
                playerInSightRange = false;
            }

        }
    }
}
