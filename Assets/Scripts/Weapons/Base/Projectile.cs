using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 15f;
    public float lifetime = 3f;

    [Header("Visual Correction")]
    public float rotationOffset = 0f;

    [Header("Frozen Bonus")]
    public bool  isIceProjectile       = false;
    public float frozenDamageMultiplier = 2f;

    protected HitData _data;
    protected bool _hasHit = false;
    protected Vector3 _moveDir;

    public virtual void Setup(HitData data)
    {
        _data = data;
        _moveDir = transform.right;
        transform.Rotate(0, 0, rotationOffset);
        
        Destroy(gameObject, lifetime);
    }

    protected virtual void Update()
    {
        transform.position += _moveDir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            _hasHit = true;

            HitData finalData = _data;
            if (!isIceProjectile && enemy.TryGetComponent(out FreezeStatus _))
                finalData.Damage = Mathf.RoundToInt(_data.Damage * frozenDamageMultiplier);

            enemy.TakeDamage(finalData);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
