using UnityEngine;

// O inimigo se aproxima até a distância de ataque, para, e atira periodicamente.
public class EnemyRanged : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed      = 1.5f;
    public float attackRange    = 6f;    // distância para parar e atirar
    public float minimumRange   = 3f;    // recua se o player chegar muito perto

    [Header("Attack")]
    public GameObject projectilePrefab;  // arraste o prefab do projétil inimigo aqui
    public float attackRate     = 2.0f;  // tiros por segundo (1/attackRate = intervalo)
    public int   damage         = 1;     // dano que o projétil causa ao player

    private Transform      _playerTransform;
    private Rigidbody2D    _rb;
    private float          _nextAttackTime;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        Vector2 toPlayer    = (Vector2)_playerTransform.position - _rb.position;
        float   distance    = toPlayer.magnitude;
        Vector2 direction   = toPlayer.normalized;

        // Rotaciona para encarar o player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rb.rotation = angle;

        if (distance > attackRange)
        {
            // Fora do alcance: avança
            _rb.linearVelocity = direction * moveSpeed;
        }
        else if (distance < minimumRange)
        {
            // Muito perto: recua
            _rb.linearVelocity = -direction * moveSpeed;
        }
        else
        {
            // Dentro do alcance: para e atira
            _rb.linearVelocity = Vector2.zero;
            TryShoot(direction);
        }
    }

    private void TryShoot(Vector2 direction)
    {
        if (Time.time < _nextAttackTime) return;
        _nextAttackTime = Time.time + attackRate;

        if (projectilePrefab == null) return;

        // Spawna o projétil levemente à frente do inimigo
        Vector2 spawnPos = _rb.position + direction * 0.6f;
        GameObject bullet = Instantiate(projectilePrefab, spawnPos, transform.rotation);

        if (bullet.TryGetComponent(out EnemyProjectile ep))
        {
            ep.Setup(damage, direction);
        }
    }
}
