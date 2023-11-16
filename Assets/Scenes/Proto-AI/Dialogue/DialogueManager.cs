using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    private TextMeshProUGUI nameText;
    private TextMeshProUGUI dialogueText;
    public bool endDialogue;
    public float typingSpeed = 0.05f; // Adjust this value to change the speed
    public bool isTyping = false;


    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        nameText = Global.instance.sceneTree.Get("Main Canvas/Dialogue Box/Name").GetComponent<TextMeshProUGUI>();
        dialogueText = Global.instance.sceneTree.Get("Main Canvas/Dialogue Box/InteractText").GetComponent<TextMeshProUGUI>();
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

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        isTyping = true;
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed); // Wait between each character
            if (!isTyping) break;
        }
        isTyping = false;
        dialogueText.text = sentence; // Ensure full sentence is displayed
    }

    public void CompleteSentence()
    {
        isTyping = false;
    }
    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        endDialogue = true;
    }
}
