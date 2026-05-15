using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [SerializeField] private LevelData levelData;
    [SerializeField] private WalletData walletData;
    [SerializeField] private string casinoSceneName = "CassinoScene";
    [SerializeField] private string startMenuSceneName = "StartMenu";
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private List<DialogueData> gameVictoryDialogues;
    [SerializeField] private List<DialogueData> beforeFirstRoundCutsceneDialogue;
    [SerializeField] private List<DialogueData> afterFirstRoundCutscene;
    [SerializeField] private List<DialogueData> gameOverDialogues;
    
    [Header("Inventory Settings")]
    [SerializeField] private WeaponInventory playerInventory;
    [SerializeField] private GameObject startingWeaponPrefab;

    [Header("Tutorial")]
    [SerializeField] private string tutorialSceneName = "TutorialScene";
    [SerializeField] private List<GameObject> tutorialWeaponPrefabs;
    [SerializeField] private List<DialogueData> introSequence;

    private bool _firstRoundCutscenePlayed = false;
    private List<DialogueData> _currentSequence;
    private int _currentSequenceIndex;
    private System.Action _onSequenceComplete;

    // Garante reset dos estáticos ao entrar no Play Mode mesmo com Reload Domain desabilitado
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { Instance = null; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDeath;
        SceneManager.sceneLoaded  += OnSceneLoaded;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
        SceneManager.sceneLoaded  -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Atualiza a referência se ela foi destruída junto com a cena anterior
        if (dialogueUI == null)
        {
            var dm = FindObjectOfType<DialogueManager>();
            if (dm != null) dialogueUI = dm.GetDialogueUI();
        }
    }

    private void HandlePlayerDeath()
    {
        if (gameOverDialogues != null && gameOverDialogues.Count > 0)
        {
            PlayDialogueSequence(gameOverDialogues, () => 
            {
                SceneManager.LoadScene(startMenuSceneName);
            });
        }
        else
        {
            SceneManager.LoadScene(startMenuSceneName);
        }
    }

    public void InitializeSession()
    {
        levelData.ResetProgress();
        if (walletData != null) walletData.coins = 0;
        _firstRoundCutscenePlayed = false;
        
        if (playerInventory != null)
        {
            playerInventory.ClearInventory();
            if (startingWeaponPrefab != null)
            {
                playerInventory.AddWeapon(startingWeaponPrefab);
            }
        }
        
        // Shuffle species
        levelData.shuffledSpeciesList = levelData.allAvailableSpecies
            .OrderBy(x => System.Guid.NewGuid())
            .ToList();
    }

    public void LoadNextLevel()
    {
        if (levelData.shuffledSpeciesList == null || levelData.shuffledSpeciesList.Count == 0)
        {
            InitializeSession();
        }

        if (levelData.roundInCurrentSpecies > 3)
        {
            levelData.currentSpeciesIndex++;
            levelData.roundInCurrentSpecies = 1;
        }

        if (levelData.currentSpeciesIndex >= levelData.shuffledSpeciesList.Count)
        {
            PlayDialogueSequence(gameVictoryDialogues, () => 
            {
                SceneManager.LoadScene(startMenuSceneName);
            });
            return;
        }

        string targetScene = levelData.shuffledSpeciesList[levelData.currentSpeciesIndex].sceneName;
        SceneManager.LoadScene(targetScene);
    }

    public void PlayDialogueSequence(List<DialogueData> sequence, System.Action onComplete)
    {
        if (sequence == null || sequence.Count == 0 || dialogueUI == null)
        {
            onComplete?.Invoke();
            return;
        }

        _currentSequence = sequence;
        _currentSequenceIndex = 0;
        _onSequenceComplete = onComplete;

        PlayNextInSequence();
    }

    private void PlayNextInSequence()
    {
        if (_currentSequenceIndex < _currentSequence.Count)
        {
            dialogueUI.OnDialogueEnded += PlayNextInSequence;
            dialogueUI.ShowDialogue(_currentSequence[_currentSequenceIndex]);
            _currentSequenceIndex++;
        }
        else
        {
            _currentSequence = null;
            System.Action callback = _onSequenceComplete;
            _onSequenceComplete = null;
            callback?.Invoke();
        }
    }

    public void LoadCasino()
    {
        SceneManager.LoadScene(casinoSceneName);
    }

    public void LoadTutorial()
    {
        if (playerInventory != null)
        {
            playerInventory.ClearInventory();
            foreach (var weapon in tutorialWeaponPrefabs)
            {
                if (weapon != null) playerInventory.AddWeapon(weapon);
            }
        }
        SceneManager.LoadScene(tutorialSceneName);
    }

    // Chamado ao fim do tutorial (ou diretamente pelo StartScreenUI).
    // Inicializa a sessão, toca a intro cutscene e carrega o primeiro nível.
    public void LaunchGame()
    {
        InitializeSession();

        if (introSequence != null && introSequence.Count > 0)
            PlayDialogueSequence(introSequence, LoadNextLevel);
        else
            LoadNextLevel();
    }

    // Cancela a sequência em curso e executa o callback de conclusão imediatamente.
    public void SkipCurrentSequence()
    {
        if (_currentSequence == null) return;

        _currentSequence      = null;
        _currentSequenceIndex = 0;
        System.Action callback = _onSequenceComplete;
        _onSequenceComplete   = null;

        if (dialogueUI != null)
        {
            dialogueUI.OnDialogueEnded = null; // impede PlayNextInSequence de disparar
            dialogueUI.EndDialogue();
        }

        callback?.Invoke();
    }

#if UNITY_EDITOR
    public void DebugLoadScene(string sceneName)
    {
        InitializeSession();
        SceneManager.LoadScene(sceneName);
    }
#endif

    public void HandleNormalRoundVictory()
    {
        if (!_firstRoundCutscenePlayed && afterFirstRoundCutscene != null && afterFirstRoundCutscene.Count > 0)
        {
            _firstRoundCutscenePlayed = true;
            bool hasGameplayDialogue = beforeFirstRoundCutsceneDialogue != null && beforeFirstRoundCutsceneDialogue.Count > 0;
            if (hasGameplayDialogue)
            {
                PlayDialogueSequence(beforeFirstRoundCutsceneDialogue, () =>
                {
                    PlayDialogueSequence(afterFirstRoundCutscene, LoadCasino);
                });
            }
            else
            {
                PlayDialogueSequence(afterFirstRoundCutscene, LoadCasino);
            }
            return;
        }

        SpeciesData currentSpecies = GetCurrentSpecies();
        if (currentSpecies != null && currentSpecies.normalRoundVictoryDialogue != null)
        {
            List<DialogueData> seq = new List<DialogueData> { currentSpecies.normalRoundVictoryDialogue };
            PlayDialogueSequence(seq, LoadCasino);
        }
        else
        {
            LoadCasino();
        }
    }

    public void HandleBossVictory()
    {
        bool isLastBoss = (levelData.shuffledSpeciesList != null && levelData.currentSpeciesIndex >= levelData.shuffledSpeciesList.Count - 1);
        System.Action onDialogueEnd = isLastBoss ? (System.Action)LoadNextLevel : LoadCasino;

        SpeciesData currentSpecies = GetCurrentSpecies();
        if (currentSpecies != null && currentSpecies.bossVictoryDialogues != null && currentSpecies.bossVictoryDialogues.Count > 0)
        {
            PlayDialogueSequence(currentSpecies.bossVictoryDialogues, onDialogueEnd);
        }
        else
        {
            onDialogueEnd?.Invoke();
        }
    }

    public SpeciesData GetCurrentSpecies()
    {
        if (levelData.shuffledSpeciesList != null && 
            levelData.currentSpeciesIndex < levelData.shuffledSpeciesList.Count)
        {
            return levelData.shuffledSpeciesList[levelData.currentSpeciesIndex];
        }
        return null;
    }
}
