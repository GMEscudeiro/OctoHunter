using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveSpawner : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> enemyPrefabs;
    public Transform playerTransform;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int baseEnemyCount = 5;
    public float enemiesPerWaveMultiplier = 1.5f;
    
    [Header("Spawn Rate Settings")]
    public float baseSpawnInterval = 2.0f;
    public float speedIncreasePerWave = 0.1f;
    public float minSpawnInterval = 0.3f;

    [Header("State")]
    private int _enemiesToSpawn;
    private int _enemiesAlive;
    private bool _isSpawning = false;

    void OnEnable() => Enemy.OnEnemyDied += HandleEnemyDeath;
    void OnDisable() => Enemy.OnEnemyDied -= HandleEnemyDeath;

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        _isSpawning = true;
        
        _enemiesToSpawn = Mathf.RoundToInt(baseEnemyCount * Mathf.Pow(enemiesPerWaveMultiplier, currentWave - 1));
        _enemiesAlive = _enemiesToSpawn;
        
        float currentInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - (currentWave * speedIncreasePerWave));

        Debug.Log($"Wave {currentWave} Started! Total Enemies: {_enemiesToSpawn}");
        
        StartCoroutine(SpawnWaveRoutine(currentInterval));
    }

    private IEnumerator SpawnWaveRoutine(float interval)
    {
        for (int i = 0; i < _enemiesToSpawn; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(interval);
        }
        _isSpawning = false;
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform == null) return;

        float angle = Random.Range(0, Mathf.PI * 2);
        Vector2 spawnPos = new Vector2(
            playerTransform.position.x + Mathf.Cos(angle) * 12f,
            playerTransform.position.y + Mathf.Sin(angle) * 12f
        );

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

    private void HandleEnemyDeath()
    {
        _enemiesAlive--;

        // Check if wave is over
        if (_enemiesAlive <= 0 && !_isSpawning)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        Debug.Log($"Wave {currentWave} Completed!");
        currentWave++;
        
        Invoke(nameof(StartNextWave), 3.0f);
    }
}
