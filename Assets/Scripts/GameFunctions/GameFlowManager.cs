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

            // Desvincula o Canvas de menu (filho direto do GFM) para que
            // ele fique na cena e seja destruído normalmente ao sair dela.
            // O DialogueManager deve ser um objeto RAIZ separado — não um
            // filho do GFM — para que ele gerencie sua própria persistência.
            transform.DetachChildren();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Duplicata criada ao recarregar a StartScene.
            // Libera o Canvas novo (com UI de menu fresca) para que
            // ele permaneça ativo na cena recarregada.
            transform.DetachChildren();
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
        // DialogueManager.Instance é o singleton persistente (DontDestroyOnLoad).
        // Usar Instance diretamente evita achar um duplicado temporário via FindObjectOfType.
        if (DialogueManager.Instance != null)
        {
            dialogueUI = DialogueManager.Instance.GetDialogueUI();
            Debug.Log($"[GFM] OnSceneLoaded '{scene.name}': DialogueUI via DM.Instance → {dialogueUI != null}");
        }
        else
        {
            dialogueUI = null;
            Debug.Log($"[GFM] OnSceneLoaded '{scene.name}': DialogueManager.Instance é null — sem dialogueUI.");
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
        Debug.Log($"[GFM] LoadNextLevel: speciesIndex={levelData.currentSpeciesIndex}, round={levelData.roundInCurrentSpecies}, speciesCount={levelData.shuffledSpeciesList?.Count ?? -1}");

        if (levelData.shuffledSpeciesList == null || levelData.shuffledSpeciesList.Count == 0)
        {
            Debug.LogWarning("[GFM] shuffledSpeciesList vazia! Chamando InitializeSession.");
            InitializeSession();
        }

        if (levelData.roundInCurrentSpecies > 3)
        {
            levelData.currentSpeciesIndex++;
            levelData.roundInCurrentSpecies = 1;
        }

        if (levelData.currentSpeciesIndex >= levelData.shuffledSpeciesList.Count)
        {
            Debug.Log("[GFM] Todos os bosses derrotados → Victory!");
            PlayDialogueSequence(gameVictoryDialogues, () => 
            {
                SceneManager.LoadScene(startMenuSceneName);
            });
            return;
        }

        string targetScene = levelData.shuffledSpeciesList[levelData.currentSpeciesIndex].sceneName;
        Debug.Log($"[GFM] Carregando cena: {targetScene}");
        SceneManager.LoadScene(targetScene);
    }

    public void PlayDialogueSequence(List<DialogueData> sequence, System.Action onComplete)
    {
        if (sequence == null || sequence.Count == 0 || dialogueUI == null)
        {
            Debug.Log($"[GFM] PlayDialogueSequence: pulando (sequence null/vazio ou dialogueUI null). dialogueUI={dialogueUI != null}");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[GFM] PlayDialogueSequence: iniciando sequência com {sequence.Count} item(s).");
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

    public void ReturnToStartMenu()
    {
        Debug.Log("[GFM] ReturnToStartMenu chamado.");
        // Limpa qualquer sequência de diálogo em andamento
        _currentSequence = null;
        _currentSequenceIndex = 0;
        _onSequenceComplete = null;
        if (dialogueUI != null) dialogueUI.OnDialogueEnded = null;
        SceneManager.LoadScene(startMenuSceneName);
    }

    // Chamado ao fim do tutorial (ou diretamente pelo StartScreenUI).
    // Inicializa a sessão, toca a intro cutscene e carrega o primeiro nível.
    public void LaunchGame()
    {
        Debug.Log($"[GFM] LaunchGame chamado. dialogueUI={dialogueUI != null}, introSequence count={introSequence?.Count ?? 0}");
        InitializeSession();

        if (introSequence != null && introSequence.Count > 0)
        {
            Debug.Log("[GFM] Iniciando intro cutscene...");
            PlayDialogueSequence(introSequence, LoadNextLevel);
        }
        else
        {
            Debug.Log("[GFM] Sem intro cutscene, carregando próximo nível diretamente.");
            LoadNextLevel();
        }
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
