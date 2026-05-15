using UnityEngine;

// Prefab precisa de: SpriteRenderer, Rigidbody2D (Kinematic), Collider2D (Is Trigger)
public class BazookaProjectile : Projectile
{
    [Header("Sound")]
    public AudioClip explosionSound;
    [Range(0f, 1f)] public float explosionSoundVolume = 1f;

    private float    _explosionRadius;
    private int      _explosionDamage;
    private bool     _hasExploded;

    public void Setup(HitData hitData, float projectileSpeed, float explosionRadius, int explosionDamage)
    {
        base.Setup(hitData);
        speed = projectileSpeed;
        _explosionRadius = explosionRadius;
        _explosionDamage = explosionDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit || _hasExploded) return;

        if (other.TryGetComponent(out Enemy _) || other.CompareTag("Wall"))
        {
            _hasHit = true; // Use base hit flag
            Explode();
        }
    }

    private void Explode()
    {
        _hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                HitData aoeHit = new HitData
                {
                    Damage   = _explosionDamage,
                    Attacker = _data.Attacker,
                    Effect   = _data.Effect
                };
                enemy.TakeDamage(aoeHit);
            }
        }

        AudioManager.Instance?.PlaySFX(explosionSound, explosionSoundVolume);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
