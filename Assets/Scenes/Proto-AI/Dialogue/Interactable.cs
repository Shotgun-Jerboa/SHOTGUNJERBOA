using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogue1 dialogue;
    public Dialogue1 secondDialogue;

    public void TriggerDialogue()
    {
    }

    public void TriggerDialogue(int interactionCount)
    {

        if (interactionCount == 0)
        {
            // Display first dialogue
            FindAnyObjectByType<DialogueManager>().StartDialogue(dialogue);
        }
        else
        {
            // Display second dialogue for all subsequent interactions
            FindAnyObjectByType<DialogueManager>().StartDialogue(secondDialogue);
        }
    }
}
