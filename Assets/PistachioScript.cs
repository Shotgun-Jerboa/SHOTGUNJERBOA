using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistachioScript : MonoBehaviour
{
    private ShotgunMain shotgunsRef;

    void Start()
    {
        shotgunsRef = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
            shotgunsRef.ammo++;
        }
    }
}
