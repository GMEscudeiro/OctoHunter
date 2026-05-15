using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optional: DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public DialogueUI GetDialogueUI() => dialogueUI;

    public void StartDialogue(DialogueData data)
    {
        if (dialogueUI == null)
        {
            Debug.LogError("[DialogueManager] DialogueUI reference is missing!");
            return;
        }

        dialogueUI.ShowDialogue(data);
    }

    public void StopDialogue()
    {
        dialogueUI.EndDialogue();
    }
}
