using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Start_Events : MonoBehaviour
{
    void Start()
    {
        ShotgunMain main = GameObject.Find("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();

        //main.addGun("Weapon_ProtoShotgunA");
        //main.addGun("Weapon_ProtoShotgunB");
    }
}
