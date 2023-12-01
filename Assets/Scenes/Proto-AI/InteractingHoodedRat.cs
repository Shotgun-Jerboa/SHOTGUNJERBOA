using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractingHoodedRat : NPCInteract
{
    [SerializeField] GameObject shotgun;
    Animator animator;

    protected override void Start()
    {
        base.Start(); // Call base class start method
        animator = GetComponent<Animator>();
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
                    animator.SetBool("GiveGun", true);
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
        if (shotgun != null)
        {
            shotgun.SetActive(true);
        }
    }

}
