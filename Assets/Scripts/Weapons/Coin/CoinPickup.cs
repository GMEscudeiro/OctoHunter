using UnityEngine;

// O prefab precisa de: SpriteRenderer, Collider2D com "Is Trigger" marcado.
public class CoinPickup : MonoBehaviour
{
    [Header("Value")]
    public int value = 1;

    [Header("Sound")]
    public AudioClip collectSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    private Transform pullTarget;
    public float pullSpeed = 15f;

    public void StartPull(Transform target)
    {
        pullTarget = target;
    }

    private void Update()
    {
        if (pullTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, pullTarget.position, pullSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerWallet wallet))
        {
            wallet.AddCoins(value);
            AudioManager.Instance?.PlaySFX(collectSound, soundVolume);
            Destroy(gameObject);
        }
    }
}
