using UnityEngine;

// Adicione este script no prefab de qualquer inimigo que deva dropar moedas.
// Ele escuta o momento da morte pelo Enemy.cs e instancia o prefab da moeda.
public class CoinDrop : MonoBehaviour
{
    [Header("Drop Settings")]
    public GameObject coinPrefab;          // prefab da moeda (com CoinPickup)
    public int        minCoins = 1;
    public int        maxCoins = 3;

    [Header("Scatter")]
    public float scatterRadius = 0.5f;     // raio do espalhamento ao dropar

    private Enemy _enemy;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    void OnEnable()
    {
        // Patch: Enemy.cs dispara OnEnemyDied (static) mas não passa posição.
        // Usamos OnDestroy para garantir execução no momento certo.
    }

    // OnDestroy é chamado quando Enemy.Die() chama Destroy(gameObject)
    void OnDestroy()
    {
        // Só dropa se a cena ainda estiver rodando (evita drop ao fechar o jogo)
        if (!gameObject.scene.isLoaded) return;
        if (coinPrefab == null) return;

        int amount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < amount; i++)
        {
            Vector2 offset = Random.insideUnitCircle * scatterRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}
