using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 15f;
    public float lifetime = 3f;

    private HitData _data;
    private bool _hasHit = false;

    public void Setup(HitData data)
    {
        _data = data;
        
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit) return;

        if (other.TryGetComponent(out Enemy enemy))
        {
            _hasHit = true;
            
            enemy.TakeDamage(_data);
            
            Destroy(gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
