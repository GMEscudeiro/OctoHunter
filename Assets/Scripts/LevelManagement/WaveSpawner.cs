using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Estrutura seguindo o GDD:
// - Cada Round possui 3 Hordas de inimigos comuns
// - A cada 3 Rounds ocorre um Boss Round
// - Entre os Rounds o jogador visita o cassino
// - Ao vencer o Boss Round do Deserto, vai para a SpiderScene

public class WaveSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public float spawnRadius       = 12f;
    public float timeBetweenHordes = 3f;
    public float timeBetweenRounds = 2f;

    [Header("Horde Scaling (linear)")]
    public int baseEnemyCount      = 5;
    public int enemiesPerRound     = 2;
    public int enemiesPerHorde     = 1;

    [Header("Data")]
    public WalletData walletData;
    public LevelData  levelData;

    [Header("Spawn Interval Scaling")]
    public float baseSpawnInterval = 1.5f;
    public float minSpawnInterval  = 0.4f;
    public float intervalReduction = 0.05f;

    [Header("State (read-only)")]
    [SerializeField] private int  _currentRound = 1;
    [SerializeField] private int  _currentHorde = 1;
    [SerializeField] private bool _isBossRound  = false;

    private Transform _playerTransform;
    private bool      _isSpawning = false;
    private int       _enemiesAlive = 0;
    public int EnemiesAlive => _enemiesAlive;

    private SpeciesData _currentSpecies;

    // Eventos para outros sistemas escutarem
    public static event System.Action<int, int> OnHordeStarted;   // round, horde
    public static event System.Action<int>      OnRoundCompleted; // round
    public static event System.Action           OnBossRoundStarted;

    void OnEnable()  => Enemy.OnEnemyDied += HandleEnemyDeath;
    void OnDisable() => Enemy.OnEnemyDied -= HandleEnemyDeath;

    void Start()
    {
        FindPlayer();

        if (GameFlowManager.Instance == null)
        {
            Debug.LogError("[WaveSpawner] GameFlowManager não encontrado! Certifique-se de que ele existe na cena inicial.");
            return;
        }

        _currentSpecies = GameFlowManager.Instance.GetCurrentSpecies();
        if (_currentSpecies == null)
        {
            Debug.LogError("[WaveSpawner] Nenhuma espécie atual encontrada no GameFlowManager!");
            return;
        }

        _currentRound = levelData.roundInCurrentSpecies;
        StartCoroutine(StartRound());
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;
    }

    private IEnumerator StartRound()
    {
        // Round 3 de cada espécie é o Boss
        _isBossRound = (_currentRound == 3);

        if (_isBossRound)
        {
            Debug.Log($"[WaveSpawner] BOSS ROUND! {_currentSpecies.speciesName}");
            OnBossRoundStarted?.Invoke();
            yield return StartCoroutine(RunBossRound());
        }
        else
        {
            Debug.Log($"[WaveSpawner] Round {_currentRound} iniciado (3 hordas)");
            yield return StartCoroutine(RunNormalRound());
        }

        // Round concluído
        OnRoundCompleted?.Invoke(_currentRound);
        
        // Update data
        levelData.roundInCurrentSpecies++;
        levelData.totalGlobalRounds++;

        yield return new WaitForSeconds(timeBetweenRounds);

        if (_isBossRound)
        {
            GameFlowManager.Instance.HandleBossVictory();
        }
        else
        {
            // Vai ao cassino entre rounds normais
            GameFlowManager.Instance.LoadCasino();
        }
    }

    private IEnumerator RunNormalRound()
    {
        for (int horde = 1; horde <= 3; horde++)
        {
            _currentHorde = horde;
            OnHordeStarted?.Invoke(_currentRound, horde);

            int enemyCount = CalculateHordeSize(horde);
            float interval = CalculateSpawnInterval();

            yield return StartCoroutine(SpawnHorde(enemyCount, interval));

            yield return new WaitUntil(() => _enemiesAlive <= 0 && !_isSpawning);

            if (horde < 3)
                yield return new WaitForSeconds(timeBetweenHordes);
        }
    }

    private IEnumerator RunBossRound()
    {
        if (_currentSpecies.bossPrefab == null)
        {
            Debug.LogWarning("[WaveSpawner] BossPrefab não definido na espécie! Pulando.");
            yield break;
        }

        if (_playerTransform == null) FindPlayer();
        if (_playerTransform == null) yield break;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 spawnPos = (Vector2)_playerTransform.position +
                           new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        
        _enemiesAlive = 1;
        Instantiate(_currentSpecies.bossPrefab, spawnPos, Quaternion.identity);

        yield return new WaitUntil(() => _enemiesAlive <= 0);
    }

    private IEnumerator SpawnHorde(int count, float interval)
    {
        _isSpawning   = true;
        _enemiesAlive = count;

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(interval);
        }

        _isSpawning = false;
    }

    private void SpawnEnemy()
    {
        if (_playerTransform == null) FindPlayer();
        if (_playerTransform == null || _currentSpecies.commonEnemies.Count == 0) return;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 spawnPos = (Vector2)_playerTransform.position +
                           new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        GameObject prefab = _currentSpecies.commonEnemies[Random.Range(0, _currentSpecies.commonEnemies.Count)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private void HandleEnemyDeath()
    {
        if (_enemiesAlive > 0)
            _enemiesAlive--;
    }

    private int CalculateHordeSize(int hordeIndex)
    {
        // Scaling usa o round GLOBAL para dificuldade progressiva
        return baseEnemyCount
             + (levelData.totalGlobalRounds - 1) * enemiesPerRound
             + (hordeIndex - 1) * enemiesPerHorde;
    }

    private float CalculateSpawnInterval()
    {
        return Mathf.Max(minSpawnInterval,
               baseSpawnInterval - (levelData.totalGlobalRounds - 1) * intervalReduction);
    }
}
