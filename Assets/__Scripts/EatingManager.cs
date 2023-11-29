using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatingManager : MonoBehaviour
{
    SettingVars settingVars;
    ShotgunMain shotgunMain;

    List<GameObject> weaponObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        settingVars = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        shotgunMain = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();



    }

    // Update is called once per frame
    void Update()
    {
        if (settingVars.input.Gameplay.Eat.IsPressed() )
        {
           StartCoroutine(EatingAction());
            Debug.Log("Eat");
        }
    }

    IEnumerator EatingAction()
    {
        GameObject weaponsParent = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons");

        // Clear the list before adding new objects
        weaponObjects.Clear();

        // Find all child GameObjects with tag "Weapon"
        foreach (Transform child in weaponsParent.transform)
        {
            if (child.CompareTag("Weapon"))
            {
                weaponObjects.Add(child.gameObject);
            }
        }

        // Deactivate each weapon

        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
                shotgunMain.isShootingAllowed = false;
            }
        }
        yield return new WaitForSeconds(2);

        // Reactivate each weapon
        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
            {
                weapon.SetActive(true);
                shotgunMain.isShootingAllowed = true;

            }
        }
    }
}
