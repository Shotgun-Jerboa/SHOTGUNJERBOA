using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayPistachioCollectSound(Transform pistachioTransform, AudioClip audioClip)
    {
        // Play the provided audio clip at the position of the collected pistachio.
        AudioSource.PlayClipAtPoint(audioClip, pistachioTransform.position);
    }
}

