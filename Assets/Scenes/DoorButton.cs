using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public bool isPressed = false;
    Animator doorAnimator;
    [SerializeField] Animator buttonAnimator;



    private void Start()
    {

    }
    private void Update()
    {
        if (isPressed == true)
        {
            buttonAnimator.SetBool("IsPressed", true);
        }


    }
}
