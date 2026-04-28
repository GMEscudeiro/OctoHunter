using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject enemyPrefab;
    public Transform playerTransform;

    [Header("Spawn Settings")]
    public float spawnRadius = 10f;
    public float initialSpawnInterval = 2.0f;
    public float minimumSpawnInterval = 0.5f;

    [Header("Difficulty Scaling")]
    [Tooltip("How much the interval decreases every second")]
    public float difficultyFactor = 0.01f; 

    private float _currentSpawnInterval;
    private float _timeSinceStart;

    void Start()
    {
        _currentSpawnInterval = initialSpawnInterval;
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        _timeSinceStart += Time.deltaTime;
        
        _currentSpawnInterval = Mathf.Max(
            minimumSpawnInterval, 
            initialSpawnInterval - (_timeSinceStart * difficultyFactor)
        );
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_currentSpawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (playerTransform == null) return;

        Vector2 spawnPos = GetRandomPositionAroundPlayer();

        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 GetRandomPositionAroundPlayer()
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        
        float x = Mathf.Cos(angle) * spawnRadius;
        float y = Mathf.Sin(angle) * spawnRadius;

        return new Vector2(playerTransform.position.x + x, playerTransform.position.y + y);
    }

    private void OnDrawGizmos()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(playerTransform.position, spawnRadius);
        }
    }
}
