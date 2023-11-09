using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bottomless_Pit : MonoBehaviour
{

    private void Awake()
    {
        Debug.Log("Bottomlessness Exists");
        if(this.gameObject.GetComponent<Collider>() == null)
        {
            Debug.Log("collider exists");


        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.name == "Player" || other.transform.parent.name == "Player")
        {
            Debug.Log("PlayerFell");
            GameManager.Instance.PlayerAnnihilation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "Player")
        {
            Debug.Log("PlayerFell");
            GameManager.Instance.PlayerAnnihilation();
        }
    }
}
