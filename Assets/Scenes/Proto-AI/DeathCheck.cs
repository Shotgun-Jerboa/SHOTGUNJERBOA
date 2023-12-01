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
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        playerAnimator = GameObject.Find("Camera").GetComponent<Animator>();
        shotgunMain = GameObject.Find("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
        deathScreen = GameObject.Find("Main Canvas/DeathScreen").GetComponent<Animator>();
        settingVars = GameObject.Find("Settings").GetComponent<SettingVars>();
        camScript = GameObject.Find("Camera/Main Camera").GetComponent<CamScript>();
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
