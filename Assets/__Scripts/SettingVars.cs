using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SettingVars : MonoBehaviour
{
    public float CamSensitivityX;
    public float CamSensitivityY;

    public PlayerInputSystem input;

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
