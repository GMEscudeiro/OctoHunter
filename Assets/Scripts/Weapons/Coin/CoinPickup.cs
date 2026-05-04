using UnityEngine;

// O prefab precisa de: SpriteRenderer, Collider2D com "Is Trigger" marcado.
public class CoinPickup : MonoBehaviour
{
    [Header("Value")]
    public int value = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerWallet wallet))
        {
            wallet.AddCoins(value);
            Destroy(gameObject);
        }
    }
}
