using UnityEngine;
using System;

public class ShipPartPickup : MonoBehaviour
{
    [SerializeField] private LevelData levelData;
    
    // Evento para avisar o sistema de fluxo que uma peça foi coletada
    public static event Action OnPartCollected;

    private bool _collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_collected) return;

        if (collision.CompareTag("Player"))
        {
            _collected = true;
            
            if (levelData != null)
            {
                levelData.collectedShipPartsCount++;
            }
            else
            {
                Debug.LogWarning("[ShipPartPickup] LevelData não referenciado no prefab da Parte da Nave!");
            }

            OnPartCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
