using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private DialogueUI dialogueUI;

    // Garante reset do singleton ao entrar no Play Mode mesmo com Reload Domain desabilitado
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { Instance = null; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Persiste este GameObject (e todos os seus filhos — Canvas de diálogo,
            // EventSystem, LevelDialogueController) em todas as cenas.
            // Para isso funcionar, o DialogueManager deve ser um objeto RAIZ
            // na StartScene, FORA da hierarquia do GameFlowManager.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destrói o duplicado criado ao recarregar a StartScene.
            // Como os filhos (Canvas de diálogo) fazem parte deste objeto,
            // eles também são destruídos automaticamente.
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
