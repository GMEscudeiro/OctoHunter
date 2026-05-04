using UnityEngine;
using System.Collections;

public class BossAbilityVenomPool : MonoBehaviour
{
    [Header("Settings")]
    public GameObject venomPoolPrefab;  // prefab da poça de veneno
    public int        poolCount  = 3;   // quantas poças spawna por uso
    public float      spreadRadius = 3f;// raio de espalhamento ao redor do player

    public void Activate(Transform playerTransform)
    {
        for (int i = 0; i < poolCount; i++)
        {
            Vector2 offset   = Random.insideUnitCircle * spreadRadius;
            Vector3 spawnPos = playerTransform.position + new Vector3(offset.x, offset.y, 0);
            Instantiate(venomPoolPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("[SnakeBoss] Habilidade: Poça de Veneno!");
    }
}
