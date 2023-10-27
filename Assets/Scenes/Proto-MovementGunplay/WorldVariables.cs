using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVariables : MonoBehaviour
{
    // Meant for settings NOT controlled by the player such as movement speed, gravity, gun force, etc.
    // Mostly for debug purposes

    // Commented out variables are yet to be implemented

    [Header("Speed Properties")]
    public float moveSpeed; // Last set to 8
    public float sprintSpeedMultiplier; // Last set to 1.5f
    public float baseAcceleration; // Last set to 75
    public float airTimeMultiplier; // Last set to 1.2

    [Header("Jump Info")]
    public float hopHeight; // Last set to 0.5f
    public float hopTimeToPeak; // Last set to 0.3f
    public float sprintHopHeight; // Last set to 1f
    public float sprintTimeToPeak; // Last set to 0.2f
    public float jumpHeight; // Last set to 2.5f
    public float jumpTimeToPeak; // Last set to 0.3f
    public float jumpWaitForGround; // Last set to 0.65f
    public float jumpGroundThreshold; // Last set to 0.5f

    [Header("Player Physics Properties")]
    public float groundDrag; // Last set to 10
    public float playerGravityMultiplier; // Last set to 2.5f
}
