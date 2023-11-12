using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //[SerializeField] GameObject leftHand;
    //[SerializeField] GameObject rightHand;
    // private GameObject player;
    private ShotgunMain shotgunMain;
    //[SerializeField] Shotgun right;

    private TextMeshProUGUI healthText;
    private TextMeshProUGUI ammoText;

    //[SerializeField] GameObject dot;

    // Start is called before the first frame update
    void Start()
    {
        // player = Global.instance.sceneTree.Get("Player");
        shotgunMain = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();

        Global.SceneHeirarchy children = new(gameObject);

        healthText = children.Get("Health/Text (TMP)").GetComponent<TextMeshProUGUI>();
        ammoText = children.Get("Ammo/Text (TMP)").GetComponent<TextMeshProUGUI>();

        //left = leftHand.GetComponent(typeof(Shotgun)) as Shotgun;
        //right = rightHand.GetComponent(typeof(Shotgun)) as Shotgun;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(left.bulletsLeft == 98){
            dot.SetActive(false);
        }*/
        healthText.text = "Health: " + Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>().health;
        ammoText.text = "Ammo: " + shotgunMain.ammo;
        // if(Input.GetKey(KeyCode.P)){
        //     animator.SetTrigger("Damage");
        // }
    }
}
