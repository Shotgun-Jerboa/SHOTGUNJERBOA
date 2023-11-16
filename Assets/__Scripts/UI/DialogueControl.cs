using UnityEngine;

public class DialogueControl : MonoBehaviour
{
    public GameObject Dialogue;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Dialogue.SetActive(true);
            Dialogue.GetComponent<Dialogue>().index = 0;
            Dialogue.GetComponent<Dialogue>().textComponent.text = string.Empty;
            Dialogue.GetComponent<Dialogue>().StartDialogue();
        }
    }
}
