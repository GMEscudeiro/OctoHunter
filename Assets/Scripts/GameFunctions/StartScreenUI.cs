using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueData initialStoryDialogue;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
    }

    private void OnStartButtonClicked()
    {
        if (initialStoryDialogue != null && dialogueUI != null)
        {
            startButton.interactable = false;
            dialogueUI.OnDialogueEnded += StartGame;
            dialogueUI.ShowDialogue(initialStoryDialogue);
        }
        else
        {
            StartGame();
        }
    }

    private void StartGame()
    {
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
