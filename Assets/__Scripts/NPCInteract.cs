using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] protected AudioSource DialogueMusic;
    Transform AudioManager;
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
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();
        playerRb = GameObject.Find("Player").GetComponent<Rigidbody>();
        dialogueManager = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();
        setting = GameObject.Find("Settings").GetComponent<SettingVars>();
        DialogueMusic = GameObject.Find("DialogueManager").GetComponent<AudioSource>();
        DialogueMusic.Play();
        DialogueMusic.Pause();
        AudioManager = GameObject.Find("Audio Manager").GetComponent<Transform>();


    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + interactZoneOffset, zoneSize / 2, Quaternion.identity, playerLayer);
        bool isCurrentlyInZone = hitColliders.Length > 0;

        if (isCurrentlyInZone && !isPlayerInZone)
        {
            // Player just entered the zone
            isPlayerInZone = true;
            DialogueMusic.Play();
            AudioManager.gameObject.SetActive(false);
        }

        else if (!isCurrentlyInZone && isPlayerInZone)
        {
            // Player just left the zone
            isPlayerInZone = false;
            hasTalk = false; // Reset the flag when the player leaves the zone
            DialogueMusic.Pause();
            AudioManager.gameObject.SetActive(true);

        }

        if (isPlayerInZone && !hasTalk)
        {
            AudioManager.gameObject.SetActive(false);
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
        Gizmos.DrawWireCube(transform.position + interactZoneOffset, zoneSize);
    }

}
