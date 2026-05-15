using UnityEngine;

// Projétil disparado pelo EnemyRanged.
// Adicione este script no prefab do projétil inimigo.
// O prefab precisa de: SpriteRenderer, Rigidbody2D (Kinematic), Collider2D (Is Trigger).
public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed    = 8f;
    public float lifetime = 4f;

    [Header("Special Effects")]
    public bool  appliesParalysis  = false;
    public float paralyzeDuration  = 2f;

    private int     _damage;
    private Vector2 _direction;
    private bool    _hasHit;

    [Header("Visual Correction")]
    public float rotationOffset = 0f;

    public void Setup(int damage, Vector2 direction)
    {
        _damage    = damage;
        _direction = direction.normalized;
        transform.Rotate(0,0,rotationOffset);
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null && !playerHealth.IsDead)
        {
            _hasHit = true;
            if (_damage > 0) playerHealth.TakeDamage(_damage);

            if (appliesParalysis)
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                if (controller == null) controller = other.GetComponentInParent<PlayerController>();
                controller?.Paralyze(paralyzeDuration);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
