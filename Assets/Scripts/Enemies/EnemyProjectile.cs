using UnityEngine;

// Projétil disparado pelo EnemyRanged.
// Adicione este script no prefab do projétil inimigo.
// O prefab precisa de: SpriteRenderer, Rigidbody2D (Kinematic), Collider2D (Is Trigger).
public class EnemyProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed    = 8f;
    public float lifetime = 4f;

    private int     _damage;
    private Vector2 _direction;
    private bool    _hasHit;

    public void Setup(int damage, Vector2 direction)
    {
        _damage    = damage;
        _direction = direction.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(_direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            _hasHit = true;
            playerHealth.TakeDamage(_damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
