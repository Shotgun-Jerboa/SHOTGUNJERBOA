using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    
    public Animator anim;

    public GameObject Button;

    public lightonoff script;

    // Start is called before the first frame update
    void Start()
    {
        Animator anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
