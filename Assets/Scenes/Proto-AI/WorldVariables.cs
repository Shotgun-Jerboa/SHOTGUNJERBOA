using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVariables : MonoBehaviour
{
    // Meant for settings NOT controlled by the player such as movement speed, gravity, gun force, etc.
    // Mostly for debug purposes

    // Commented out variables are yet to be implemented

    [Header("Speed Properties")]
    public float moveSpeed;
    //public float sprintSpeedMultiplier;
    //public float hopSpeedMultiplier;
    //public float airSpeedMultiplier;

    [Header("Jump Info\n(Disregard the \"height\"\n This is more like starting velocity)")]
    public float hopHeight;
    //public float sprintHopHeight;
    //public float jumpHeight;

    [Header("Player Physics Properties")]
    public float groundDrag;
}
