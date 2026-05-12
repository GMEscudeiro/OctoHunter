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

    public void InitializeSession()
    {
        levelData.ResetProgress();
        if (walletData != null) walletData.coins = 0;
        
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
        levelData.collectedShipPartsCount++;
        
        SpeciesData currentSpecies = GetCurrentSpecies();
        if (currentSpecies != null && currentSpecies.bossVictoryDialogues != null && currentSpecies.bossVictoryDialogues.Count > 0)
        {
            PlayDialogueSequence(currentSpecies.bossVictoryDialogues, LoadCasino);
        }
        else
        {
            LoadCasino();
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
