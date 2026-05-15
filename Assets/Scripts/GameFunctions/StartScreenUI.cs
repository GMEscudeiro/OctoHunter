using UnityEngine;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private DialogueData initialStoryDialogue;

    [SerializeField] private GameObject menuVisuals;

#if UNITY_EDITOR
    [Header("Debug - Pular para Cena (somente Editor)")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Button debugSnakeButton;
    [SerializeField] private Button debugSpiderButton;
    [SerializeField] private Button debugScorpionButton;
    [SerializeField] private Button debugCasinoButton;
#endif

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        if (tutorialButton != null)
        {
            tutorialButton.onClick.AddListener(OnTutorialButtonClicked);
        }
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipButtonClicked);
            skipButton.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        SetupDebugButtons();
#endif
    }

#if UNITY_EDITOR
    private void SetupDebugButtons()
    {
        if (debugPanel != null) debugPanel.SetActive(true);

        debugSnakeButton?  .onClick.AddListener(() => DebugLoad("SnakeScene"));
        debugSpiderButton? .onClick.AddListener(() => DebugLoad("SpiderScene"));
        debugScorpionButton?.onClick.AddListener(() => DebugLoad("ScorpionScene"));
        debugCasinoButton? .onClick.AddListener(() => DebugLoad("CassinoScene"));
    }

    private void DebugLoad(string sceneName)
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.DebugLoadScene(sceneName);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
#endif

    private void OnTutorialButtonClicked()
    {
        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.LoadTutorial();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("TutorialScene");
    }

    private void OnStartButtonClicked()
    {
        startButton.interactable = false;

        if (menuVisuals != null)
            menuVisuals.SetActive(false);

        if (skipButton != null)
            skipButton.gameObject.SetActive(true);

        if (GameFlowManager.Instance != null)
            GameFlowManager.Instance.LaunchGame();
        else
            Debug.LogError("[StartScreenUI] GameFlowManager não encontrado!");
    }

    private void OnSkipButtonClicked()
    {
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);

        GameFlowManager.Instance?.SkipCurrentSequence();
    }
}
