using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSound : MonoBehaviour
{
    Animator animator;
    public List<AudioClip> soundCueList;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = Global.instance.sceneTree.Get("Audio Manager/SoundEffect").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySoundCueEffect()
    {
        // Check if there are any sound cues in the list
        if (soundCueList.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, soundCueList.Count);
            audioSource.clip = soundCueList[randomIndex]; // Set the audio clip
            audioSource.Play(); // Play the audio clip
        }

    }
}
