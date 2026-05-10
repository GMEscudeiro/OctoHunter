using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2.0f;
    
    private Transform _playerTransform;
    private Rigidbody2D _rb;

    [Header("Ranged Attack (Optional)")]
    public GameObject projectilePrefab;
    public float attackRate = 2.0f;
    private float _nextAttackTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        Vector2 direction = (_playerTransform.position - transform.position).normalized;

        _rb.linearVelocity = direction * moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rb.rotation = angle + 90;

        if (projectilePrefab != null && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackRate;
            Vector2 spawnPos = _rb.position + direction * 0.6f;
            GameObject bullet = Instantiate(projectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));

            if (bullet.TryGetComponent(out EnemyProjectile ep))
            {
                ep.Setup(1, direction); // Dano 1
            }
        }
    }
}
