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
        if (settingVars.input.Gameplay.Eat.IsPressed() )
        {
           StartCoroutine(EatingAction());
            EatToHealManager();
        }
    }

    void EatToHealManager()
    {
        if (settingVars.input.Gameplay.Eat.IsPressed() && !isEating)
        {
            if (shotgunMain.ammo > 0)
            {
                isEating = true;
                eatingAnimator.SetBool("Eat", true);
                player.health += 20;
                shotgunMain.ammo--;

                StartCoroutine(ResetEating());
            }
        }
    }

    IEnumerator ResetEating()
    {
        // Retrieve the duration of the "eat" animation clip
        float eatAnimationDuration = GetAnimationClipLength("Eating"); // Replace "Eat" with the exact name of your eat animation clip
        yield return new WaitForSeconds(eatAnimationDuration); // Wait for the duration of the eating animation
        eatingAnimator.SetBool("Eat", false); // Set the boolean to false to stop the animation
        isEating = false; // Reset the flag
    }

    float GetAnimationClipLength(string animationName)
    {
        RuntimeAnimatorController ac = eatingAnimator.runtimeAnimatorController; // Get the Runtime Animator Controller

        for (int i = 0; i < ac.animationClips.Length; i++) // Iterate through all animation clips
        {
            if (ac.animationClips[i].name == animationName)
            {
                return ac.animationClips[i].length; // Return the length of the animation clip
            }
        }

        return 0f; // Return 0 if the animation clip is not found
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
        shotgunMain.isShootingAllowed = false;

        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
            {
                weapon.SetActive(false);
            }
        }
        yield return new WaitForSeconds(GetAnimationClipLength("Eating"));

        // Reactivate each weapon
        foreach (var weapon in weaponObjects)
        {
            if (weapon != null)
            {
                weapon.SetActive(true);

            }
        }
        shotgunMain.isShootingAllowed = true;

    }
}
