using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingManager : MonoBehaviour
{
    SettingVars settingVars;
    ShotgunMain shotgunMain;
    PlayerScript player;

    List<GameObject> weaponObjects = new List<GameObject>();

    [Header("Eating Animation")]
    private Animator eatingAnimator;
    private bool isEating = false; // Flag to check if the player is currently eating

    // Start is called before the first frame update
    void Start()
    {
        settingVars = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        shotgunMain = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
        eatingAnimator = Global.instance.sceneTree.Get("Main Canvas/Eating Overlay").GetComponent<Animator>();
        player = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();

    }

    // Update is called once per frame
    void Update()
    {

    }


}
