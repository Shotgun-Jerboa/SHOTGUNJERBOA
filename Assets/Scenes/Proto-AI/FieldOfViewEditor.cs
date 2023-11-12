using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//This script is used to visualize the FOV
[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward,360, fov.radius);

        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.radius);

        Handles.color = Color.green;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.radius);

        // Drawing vertical limits
        Handles.color = Color.blue;
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle01 * fov.radius).normalized * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle01 * fov.radius).normalized * -fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle02 * fov.radius).normalized * fov.radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + (viewAngle02 * fov.radius).normalized * -fov.radius);

        if (fov.playerInSightRange)
        {
            Handles.color = Color.green;
            Handles.DrawLine(fov.transform.position, fov.playerRef.transform.position);
        }
    }

    Vector3 DirectionFromAngle(float eulerY, float angleIndegree)
    {
        angleIndegree += eulerY;

        return new Vector3(Mathf.Sin(angleIndegree*Mathf.Deg2Rad),
            0,
            Mathf.Cos(angleIndegree*Mathf.Deg2Rad));
    }
}
