using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistachioScript : MonoBehaviour
{
    public int ammoValue = 1;
    private ShotgunMain shotgunsRef;
    public List<AudioClip> collectedAudioClips;

    void Start()
    {
        shotgunsRef = GameObject.Find("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // Play a random audio clip
            if (collectedAudioClips.Count > 0)
            {
                int randomIndex = Random.Range(0, collectedAudioClips.Count);
                AudioSource.PlayClipAtPoint(collectedAudioClips[randomIndex], transform.position);
            }

            // Destroy the GameObject
            Destroy(gameObject);
            shotgunsRef.ammo+= ammoValue;
        }
    }
}
