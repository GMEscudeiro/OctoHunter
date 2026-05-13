using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueData initialStoryDialogue;

    [SerializeField] private GameObject menuVisuals;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipButtonClicked);
            skipButton.gameObject.SetActive(false); // Inicia oculto
        }
    }

    private void OnStartButtonClicked()
    {
        if (initialStoryDialogue != null && dialogueUI != null)
        {
            startButton.interactable = false;
            
            // Oculta apenas os visuais do menu, em vez do objeto inteiro,
            // para não inativar acidentalmente o objeto do DialogueUI (caso seja filho).
            if (menuVisuals != null)
            {
                menuVisuals.SetActive(false);
            }

            if (skipButton != null)
            {
                skipButton.gameObject.SetActive(true);
            }

            dialogueUI.OnDialogueEnded += StartGame;
            dialogueUI.ShowDialogue(initialStoryDialogue);
        }
        else
        {
            StartGame();
        }
    }

    private void OnSkipButtonClicked()
    {
        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnded -= StartGame; // Remove evento para não chamar o StartGame 2 vezes
            dialogueUI.EndDialogue(); // Para e esconde o diálogo
        }
        
        StartGame();
    }

    private void StartGame()
    {
        if (skipButton != null)
        {
            skipButton.gameObject.SetActive(false); // Esconde se o diálogo acabar normalmente
        }

        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.InitializeSession();
            GameFlowManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("[StartScreenUI] GameFlowManager não encontrado!");
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }
}
