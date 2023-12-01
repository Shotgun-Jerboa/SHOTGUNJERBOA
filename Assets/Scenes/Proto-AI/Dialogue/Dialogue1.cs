using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue1
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
    public AudioClip[] dialogueAudioClips; // Array of audio clips, one for each sentence

}
