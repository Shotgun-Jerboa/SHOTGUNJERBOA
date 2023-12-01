using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private AudioSource dialogueAudioSource;


    private Queue<string> sentences;
    private string currentSentence;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    public bool endDialogue;
    public float typingSpeed = 0.05f; // Adjust this value to change the speed
    public bool isTyping = false;

    public event Action OnDialogueComplete;


    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        nameText = Global.instance.sceneTree.Get("Main Canvas/Dialogue Box/Cloud/Name").GetComponent<TextMeshProUGUI>();
        dialogueText = Global.instance.sceneTree.Get("Main Canvas/Dialogue Box/Cloud/InteractText").GetComponent<TextMeshProUGUI>();
        animator = Global.instance.sceneTree.Get("Main Canvas/Dialogue Box").GetComponent<Animator>();

        sentences = new Queue<string>();
    }

    public void StartDialogue(Dialogue1 dialogue)
    {
        endDialogue = false;
        animator.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        if (isTyping)
        {
            CompleteSentence();
            return;
        }

        currentSentence = sentences.Dequeue();
        Debug.Log("Displaying Sentence: " + currentSentence); // Debug log
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait between each character
        }
        isTyping = false;
        Debug.Log("Finished Typing Sentence: " + sentence); // Debug log
    }

    public void CompleteSentence()
    {
        StopAllCoroutines(); // Stop the typing coroutine
        isTyping = false;
        dialogueText.text = currentSentence; // Display the full current sentence
    }
        public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        endDialogue = true;
        OnDialogueComplete?.Invoke(); // Invoke the event
    }
}
