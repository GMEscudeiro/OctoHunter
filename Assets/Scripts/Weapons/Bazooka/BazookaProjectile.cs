using UnityEngine;

// Prefab precisa de: SpriteRenderer, Rigidbody2D (Kinematic), Collider2D (Is Trigger)
public class BazookaProjectile : MonoBehaviour
{
    private HitData  _hitData;
    private float    _speed;
    private float    _explosionRadius;
    private int      _explosionDamage;
    private GameObject _attacker;
    private bool     _hasExploded;

    public void Setup(HitData hitData, float speed, float explosionRadius, int explosionDamage, GameObject attacker)
    {
        _hitData         = hitData;
        _speed           = speed;
        _explosionRadius = explosionRadius;
        _explosionDamage = explosionDamage;
        _attacker        = attacker;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasExploded) return;

        if (other.TryGetComponent(out Enemy _) || other.CompareTag("Wall"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;

        // Dano em área — acerta todos os inimigos no raio
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                HitData aoeHit = new HitData
                {
                    Damage   = _explosionDamage,
                    Attacker = _attacker,
                    Effect   = _hitData.Effect
                };
                enemy.TakeDamage(aoeHit);
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
