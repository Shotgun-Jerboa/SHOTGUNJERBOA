using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public SettingVars settings;
    public Transform orientation;
    float xRot;
    float yRot;

    void Update()
    {
        float mouseX = settings.input.Gameplay.Look.ReadValue<Vector2>().x * Time.deltaTime * settings.CamSensitivityX;
        float mouseY = settings.input.Gameplay.Look.ReadValue<Vector2>().y * Time.deltaTime * settings.CamSensitivityY;

        yRot += mouseX;
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }
}
