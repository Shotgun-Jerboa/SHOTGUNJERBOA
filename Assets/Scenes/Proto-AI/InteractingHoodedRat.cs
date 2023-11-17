using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractingHoodedRat : MonoBehaviour
{
    [SerializeField] GameObject shotgun;
    [SerializeField] Transform interactZone;
    [SerializeField] Vector3 zoneSize;
    [SerializeField] LayerMask playerLayer;
    private Interactable interactable;
    public DialogueManager dialogueManager;

    private bool hasTalk = false;
    public bool isPlayerInZone;
    Animator animator;
    private Rigidbody playerRb;

    private PlayerScript playerScript;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
        animator = GetComponent<Animator>();
        playerScript = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();
        playerRb = Global.instance.sceneTree.Get("Player").GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(interactZone.position, zoneSize / 2, Quaternion.identity, playerLayer);
        isPlayerInZone = hitColliders.Length > 0;

        if (isPlayerInZone && !hasTalk)
        {
            //Stop Player from moving when in dialogue
            playerScript.enabled = false;
            playerRb.velocity = Vector3.zero;

            interactable.TriggerDialogue();
            hasTalk = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueManager.isTyping)
            {
                dialogueManager.CompleteSentence();
            }

            else
            {
                dialogueManager.DisplayNextSentence();
                if (dialogueManager.endDialogue)
                {
                    animator.SetBool("GiveGun", true);
                    playerScript.enabled = true;
                }
            }
        }
        if (shotgun == null || Time.timeScale ==0)
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

    void OnDrawGizmosSelected()
    {
        // Draw a wireframe box in the editor to show interact zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactZone.position, zoneSize);
    }
}
