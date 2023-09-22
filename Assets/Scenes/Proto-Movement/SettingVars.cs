using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingVars : MonoBehaviour
{
    // Meant for variables that will be affected by the player's settings

    public float CamSensitivityX;
    public float CamSensitivityY;

    public PlayerInputSystem input;
    public WorldVariables worldVars;

    [RuntimeInitializeOnLoadMethod]
    private void Awake()
    {
        input = new PlayerInputSystem();
    }

    private void OnEnable()
    {
        foreach (var action in input)
        {
            action.Enable();
        }
    }

    private void OnDisable()
    {
        foreach (var action in input)
        {
            action.Disable();
        }
    }

}
