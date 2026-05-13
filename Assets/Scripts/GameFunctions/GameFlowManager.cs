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

    private bool _firstRoundCutscenePlayed = false;
    private List<DialogueData> _currentSequence;
    private int _currentSequenceIndex;
    private System.Action _onSequenceComplete;

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
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDeath;
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
