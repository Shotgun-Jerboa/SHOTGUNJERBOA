using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractingHoodedRat : NPCInteract
{
    [SerializeField] GameObject shotgun;
    Animator animator;
    private bool hasGivenGun = false; // Flag to track if gun has been given

    protected override void Start()
    {
        base.Start(); // Call base class start method
        animator = GetComponent<Animator>();
        dialogueManager.OnDialogueComplete += HandleDialogueCompletion; // Subscribe to the event
    }

    private void HandleDialogueCompletion()
    {
        // Check if it was the first dialogue
        if (interactionCount == 1 && !hasGivenGun)
        {
            animator.SetBool("GiveGun", true);
        }
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update(); // Call base class update method

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (dialogueManager.isTyping)
            {
                dialogueManager.CompleteSentence();
            }
            else
            {
                dialogueManager.DisplayNextSentence();
                if (dialogueManager.endDialogue && interactable.IsInDialogue)
                {
                    playerScript.enabled = true;
                    interactable.EndDialogue(); // Indicate that dialogue has ended
                }
            }
        }
        if (shotgun == null || Time.timeScale == 0)
        {
            animator.SetBool("GiveGun", false);
        }
    }

    public void GiveGun()
    {
        if (shotgun != null && !hasGivenGun)
        {
            shotgun.SetActive(true);
            hasGivenGun = true; // Ensure flag is set when gun is given

        }
    }

}
