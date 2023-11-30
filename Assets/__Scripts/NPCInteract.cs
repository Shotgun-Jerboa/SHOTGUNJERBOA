using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    SettingVars setting;
    [SerializeField] protected Vector3 interactZoneOffset;
    [SerializeField] protected Vector3 zoneSize = Vector3.one;
    [SerializeField] protected LayerMask playerLayer;
    protected Interactable interactable;
    protected DialogueManager dialogueManager;
    public bool isPlayerInZone;
    protected Rigidbody playerRb;

    protected bool hasTalk = false;
    protected int interactionCount = 0;


    [SerializeField] Quaternion interactZoneRotation = Quaternion.identity;

    protected PlayerScript playerScript;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        interactable = GetComponent<Interactable>();
        playerScript = Global.instance.sceneTree.Get("Player").GetComponent<PlayerScript>();
        playerRb = Global.instance.sceneTree.Get("Player").GetComponent<Rigidbody>();
        dialogueManager = Global.instance.sceneTree.Get("DialogueManager").GetComponent<DialogueManager>();
        setting = Global.instance.sceneTree.Get("Settings").GetComponent<SettingVars>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, zoneSize / 2, Quaternion.identity, playerLayer);
        bool isCurrentlyInZone = hitColliders.Length > 0;

        if (isCurrentlyInZone && !isPlayerInZone)
        {
            // Player just entered the zone
            isPlayerInZone = true;
        }

        else if (!isCurrentlyInZone && isPlayerInZone)
        {
            // Player just left the zone
            isPlayerInZone = false;
            hasTalk = false; // Reset the flag when the player leaves the zone
        }

        if (isPlayerInZone && !hasTalk)
        {
            //Stop Player from moving when in dialogue
            playerScript.enabled = false;
            playerRb.velocity = Vector3.zero;

            interactable.TriggerDialogue(interactionCount);
            hasTalk = true;
            if (interactionCount == 0)
            {
                interactionCount++; // Increment interaction count after each interaction
            }
        }
        if (Input.GetKeyUp(KeyCode.T))
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
                    playerScript.enabled = true;
                }
            }
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position + interactZoneOffset, transform.rotation, transform.localScale);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, zoneSize);
    }

}
