using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Estrutura seguindo o GDD:
// - Cada Round possui 3 Hordas de inimigos comuns
// - A cada 3 Rounds ocorre um Boss Round
// - Entre os Rounds o jogador visita o cassino

public class WaveSpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public List<GameObject> enemyPrefabs;   // inimigos comuns
    public GameObject bossPrefab;           // prefab do boss

    [Header("Spawn Settings")]
    public float spawnRadius       = 12f;   // distância do player para spawnar
    public float timeBetweenHordes = 3f;    // segundos entre hordas do mesmo round
    public float timeBetweenRounds = 2f;    // segundos antes de ir ao cassino

    [Header("Horde Scaling (linear)")]
    public int baseEnemyCount      = 5;     // inimigos na horde 1 do round 1
    public int enemiesPerRound     = 2;     // inimigos extras adicionados por round
    public int enemiesPerHorde     = 1;     // inimigos extras adicionados por horde dentro do round

    [Header("Data")]
    public WalletData walletData;
    public LevelData  levelData;
    [Header("Spawn Interval Scaling")]
    public float baseSpawnInterval = 1.5f;
    public float minSpawnInterval  = 0.4f;
    public float intervalReduction = 0.05f; // redução por round

    [Header("State (read-only)")]
    [SerializeField] private int  _currentRound = 1;
    [SerializeField] private int  _currentHorde = 1;
    [SerializeField] private bool _isBossRound  = false;

    private Transform _playerTransform;
    private bool      _isSpawning = false;
    private int       _enemiesAlive = 0;

    private static bool _isSessionInitialized = false;

    // Eventos para outros sistemas escutarem
    public static event System.Action<int, int> OnHordeStarted;   // round, horde
    public static event System.Action<int>      OnRoundCompleted; // round
    public static event System.Action           OnBossRoundStarted;

    void OnEnable()  => Enemy.OnEnemyDied += HandleEnemyDeath;
    void OnDisable() => Enemy.OnEnemyDied -= HandleEnemyDeath;

    void Start()
    {
        FindPlayer();

        if (!_isSessionInitialized)
        {
            if (walletData != null) walletData.coins = 0;
            if (levelData != null)  levelData.currentRound = 1;
            _isSessionInitialized = true;
            Debug.Log("[WaveSpawner] Nova sessão iniciada: Moedas e Round resetados.");
        }

        if (levelData != null)
            _currentRound = levelData.currentRound;

        StartCoroutine(StartRound());
    }

    // ── Encontrar Player ─────────────────────────────────────────────
    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;
    }

    // ── Fluxo principal ──────────────────────────────────────────────
    private IEnumerator StartRound()
    {
        _isBossRound = (_currentRound % 3 == 0);

        if (_isBossRound)
        {
            Debug.Log($"[WaveSpawner] BOSS ROUND! Round {_currentRound}");
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
        Debug.Log($"[WaveSpawner] Round {_currentRound} concluído!");
        _currentRound++;

        // Aguarda antes de ir ao cassino / próximo round
        yield return new WaitForSeconds(timeBetweenRounds);

        // Aqui você pode chamar o CasinoLoader futuramente
        // Por enquanto inicia o próximo round automaticamente
        if (!_isBossRound)
        {
            // Salva o round atual e abre o cassino
            if (levelData != null) levelData.currentRound = _currentRound;
            CasinoLoader.OpenCasino("CassinoScene");
            yield break;
        }
        else
        {
            StartCoroutine(StartRound());
        }
    }

    // ── Round normal: 3 hordas ───────────────────────────────────────
    private IEnumerator RunNormalRound()
    {
        for (int horde = 1; horde <= 3; horde++)
        {
            _currentHorde = horde;
            OnHordeStarted?.Invoke(_currentRound, horde);

            int enemyCount = CalculateHordeSize(horde);
            float interval = CalculateSpawnInterval();

            Debug.Log($"[WaveSpawner] Horde {horde}/3 — {enemyCount} inimigos");

            yield return StartCoroutine(SpawnHorde(enemyCount, interval));

            // Aguarda todos morrerem antes da próxima horde
            yield return new WaitUntil(() => _enemiesAlive <= 0);
            yield return new WaitUntil(() => _enemiesAlive <= 0 && !_isSpawning);

            if (horde < 3)
                yield return new WaitForSeconds(timeBetweenHordes);
        }
    }

    // ── Boss Round ───────────────────────────────────────────────────
    private IEnumerator RunBossRound()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("[WaveSpawner] bossPrefab não atribuído! Pulando boss round.");
            yield break;
        }

        if (_playerTransform == null) FindPlayer();
        if (_playerTransform == null) yield break;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector2 spawnPos = (Vector2)_playerTransform.position +
        new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
        _enemiesAlive = 1;
        Instantiate(bossPrefab, spawnPos, Quaternion.identity);

        Debug.Log("[WaveSpawner] Boss spawnado!");
        yield return new WaitUntil(() => _enemiesAlive <= 0);
    }

    // ── Spawnar uma horde ────────────────────────────────────────────
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
        if (_playerTransform == null || enemyPrefabs.Count == 0) return;

        float   angle    = Random.Range(0f, Mathf.PI * 2f);
        Vector2 spawnPos = (Vector2)_playerTransform.position +
                           new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    // ── Morte de inimigo ─────────────────────────────────────────────
    private void HandleEnemyDeath()
    {
        if (_enemiesAlive > 0)
            _enemiesAlive--;
    }

    // ── Cálculos de scaling (linear) ─────────────────────────────────
    private int CalculateHordeSize(int hordeIndex)
    {
        // Base + bônus por round + bônus por horde dentro do round
        return baseEnemyCount
             + (_currentRound - 1) * enemiesPerRound
             + (hordeIndex  - 1) * enemiesPerHorde;
    }

    private float CalculateSpawnInterval()
    {
        return Mathf.Max(minSpawnInterval,
               baseSpawnInterval - (_currentRound - 1) * intervalReduction);
    }
}
