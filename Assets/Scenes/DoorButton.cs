using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public bool isPressed = false;
    public float moveSpeed = 5f;
    public Vector3 targetPosition;

    private Vector3 startPosition;
    private float startTime;
    private bool isMoving = false;
    Animator doorAnimator;
    Animator buttonAnimator;



    private void Start()
    {
        buttonAnimator = GetComponent<Animator>();
                startPosition = transform.position;

    }
    private void Update()
    {
        if (isPressed)
        {
            buttonAnimator.SetBool("IsPressed", true);
        }


    }
}
