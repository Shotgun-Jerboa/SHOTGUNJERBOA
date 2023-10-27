using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamToPoint : MonoBehaviour
{
    public Transform camPosition;

    void Update()
    {
        transform.position = camPosition.position;
    }
}
