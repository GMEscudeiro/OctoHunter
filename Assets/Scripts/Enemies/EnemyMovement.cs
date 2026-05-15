using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 2.0f;

    [Header("Ranged Attack (Optional)")]
    public GameObject projectilePrefab;
    public float attackRate = 2.0f;

    private Transform    _playerTransform;
    private Rigidbody2D  _rb;
    private Enemy        _enemy;
    private NavMeshAgent _agent;
    private float        _nextAttackTime;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
        _agent.updateUpAxis   = false;
    }

    void Start()
    {
        _rb    = GetComponent<Rigidbody2D>();
        _enemy = GetComponent<Enemy>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void Update()
    {
        if (_playerTransform == null || !_agent.isOnNavMesh) return;

        // Atualiza posição do agente no Update (mesmo ciclo do NavMesh)
        _agent.nextPosition = transform.position;
        _agent.SetDestination(_playerTransform.position);
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        float   speed   = moveSpeed * (_enemy != null ? _enemy.SpeedMultiplier : 1f);
        Vector2 moveDir = GetMoveDirection();

        if (moveDir.sqrMagnitude > 0.001f)
        {
            _rb.MovePosition(_rb.position + moveDir * speed * Time.fixedDeltaTime);
            _rb.rotation = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg + 90f;
        }

        if (projectilePrefab != null && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackRate;
            Vector2 toPlayer  = ((Vector2)_playerTransform.position - _rb.position).normalized;
            float   shotAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
            GameObject bullet = Instantiate(projectilePrefab, _rb.position + toPlayer * 0.6f, Quaternion.Euler(0, 0, shotAngle));
            if (bullet.TryGetComponent(out EnemyProjectile ep))
                ep.Setup(1, toPlayer);
        }
    }

    private Vector2 GetMoveDirection()
    {
        if (_agent.isOnNavMesh && _agent.hasPath && _agent.path.corners.Length > 1)
        {
            // Segue o próximo waypoint do caminho calculado pelo NavMesh
            Vector2 nextCorner = _agent.path.corners[1];
            return (nextCorner - _rb.position).normalized;
        }

        // Fallback: linha reta ao player
        return ((Vector2)_playerTransform.position - _rb.position).normalized;
    }

    void OnDisable()
    {
        if (_agent != null && _agent.isOnNavMesh)
            _agent.ResetPath();
    }
}
