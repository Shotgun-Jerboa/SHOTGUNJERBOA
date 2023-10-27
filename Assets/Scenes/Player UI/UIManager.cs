using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //[SerializeField] GameObject leftHand;
    //[SerializeField] GameObject rightHand;
    [SerializeField] GameObject player;
    [SerializeField] ShotgunMain shotgunMain;
    //[SerializeField] Shotgun right;

    public TextMeshProUGUI healthText;
    public TextMeshProUGUI ammoText;

    //[SerializeField] GameObject dot;
    public Animator animator;

    public int health = 100;
    public int ammo = 10;
    // Start is called before the first frame update
    void Start()
    {
        //left = leftHand.GetComponent(typeof(Shotgun)) as Shotgun;
        //right = rightHand.GetComponent(typeof(Shotgun)) as Shotgun;
    }

    // Update is called once per frame
    void Update()
    {
        /*if(left.bulletsLeft == 98){
            dot.SetActive(false);
        }*/
        ammo = shotgunMain.ammo;
        healthText.text = "Health: " + health;
        ammoText.text = "Ammo: " + ammo;
        if(Input.GetKey(KeyCode.P)){
            animator.SetTrigger("Damage");
        }
    }
    public void damage(){
        health--;
    }
}
