using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PistachioScript : MonoBehaviour
{
    private ShotgunMain shotgunsRef;
    public List<AudioClip> collectedAudioClips;

    void Start()
    {
        shotgunsRef = Global.instance.sceneTree.Get("Camera/Main Camera/Weapons").GetComponent<ShotgunMain>();
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
            shotgunsRef.ammo++;
        }
    }
}
