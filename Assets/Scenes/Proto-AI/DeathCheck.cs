using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathCheck : MonoBehaviour
{
    PlayerScript playerScript;
    private Animator playerAnimator;
    public CamScript camScript;
    ShotgunMain shotgunMain;
    private Animator deathScreen;
    SettingVars settingVars;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();
        playerAnimator = Global.instance.sceneTree.Get("Camera").GetComponent<Animator>();
        shotgunMain = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
        deathScreen = Global.instance.sceneTree.Get("Main Canvas/DeathScreen").GetComponent<Animator>();
        settingVars = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
        camScript = Global.instance.sceneTree.Get("Camera/Main Camera").GetComponent<CamScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.health <=0)
        {
            playerScript.enabled = false;
            camScript.enabled = false;
            playerAnimator.SetBool("isDeath", true);
            shotgunMain.enabled = false;
            StartCoroutine(PlayDeathScreen());
            if (settingVars.input.Gameplay.Restart.IsPressed())
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
    IEnumerator PlayDeathScreen()
    {
        deathScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        deathScreen.SetBool("PlayerDeath", true);
    }
}
