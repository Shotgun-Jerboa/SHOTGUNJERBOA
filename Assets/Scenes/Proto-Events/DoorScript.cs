using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    
    Animator anim;

    public DoorButton Button;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Button.GetComponent<DoorButton>().isPressed)
        {
            anim.SetBool("Open", true);
        }
    }
}
