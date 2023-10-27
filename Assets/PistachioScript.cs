using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistachioScript : MonoBehaviour
{
    
    public PlayerScriptMERGE_EVENTS playerRef;
    
    

    void Start()
    {
        PlayerScriptMERGE_EVENTS playerRef = GetComponent<PlayerScriptMERGE_EVENTS>();
        int Ammo = playerRef.Ammo;
    }

    public Text AmmoCounter;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pistachio"))
        {
            Destroy(other.gameObject);
            playerRef.Ammo++;
            AmmoCounter.text = "Ammo: " + playerRef.Ammo;
        }
    
    }
}
