using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVariables : MonoBehaviour
{
    // Meant for settings NOT controlled by the player such as movement speed, gravity, gun force, etc.
    // Mostly for debug purposes

    // Commented out variables are yet to be implemented

    [Header("Speed Properties\nIgnore the ranged multipliers they don't work yet!")]
    public float moveSpeed;
    public float sprintSpeedMultiplier;
    [Range(0.0f, 1.0f)] public float midHopMultiplier;
    [Range(0.0f, 1.0f)] public float airTimeMultiplier;

    [Header("Jump Info")]
    public float hopHeight;
    public float hopTimeToPeak;
    public float sprintHopHeight;
    public float sprintTimeToPeak;
    //public float jumpHeight;
    //public float jumpTimeToPeak;

    [Header("Player Physics Properties")]
    public float groundDrag;
}
