using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunPickup : MonoBehaviour
{
    [Header("Using Weapon Gameobject name to add the gun")]
    //IF THIS GAME OBJECT IS SHOTGUN 1 THEN GUNID is 'Weapon_ProtoShotgunA'
    //AND THIS GAME OBJECT IS SHOTGUN 2 THEN GUNID is 'Weapon_ProtoShotgunB'
    public string gunID;

    private ShotgunMain main;
    // Start is called before the first frame update
    void Start()
    {
         main = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            main.addGun(gunID);
            Destroy(gameObject);
        }
    }
}
