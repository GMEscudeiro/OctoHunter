using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyRanged : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed    = 1.5f;
    public float attackRange  = 6f;
    public float minimumRange = 3f;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public float attackRate = 2.0f;
    public int   damage     = 1;

    [Header("Visual Correction")]
    public float rotationOffset = 90f;

    private Transform    _playerTransform;
    private Rigidbody2D  _rb;
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
        _rb = GetComponent<Rigidbody2D>();
        _agent.stoppingDistance = attackRange * 0.9f;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _playerTransform = player.transform;
    }

    void Update()
    {
        if (_playerTransform == null || !_agent.isOnNavMesh) return;

        _agent.nextPosition = transform.position;

        Vector2 toPlayer = (Vector2)_playerTransform.position - (Vector2)transform.position;
        float   distance = toPlayer.magnitude;

        if (distance > attackRange)
            _agent.SetDestination(_playerTransform.position);
        else
            _agent.ResetPath();
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        Vector2 toPlayer = (Vector2)_playerTransform.position - _rb.position;
        float   distance = toPlayer.magnitude;
        Vector2 dir      = toPlayer.normalized;

        _rb.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + rotationOffset;

        if (distance > attackRange)
        {
            Vector2 moveDir = GetNavDirection(dir);
            _rb.MovePosition(_rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
        else if (distance < minimumRange)
        {
            _rb.MovePosition(_rb.position + (-dir) * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            TryShoot(dir);
        }
    }

    private Vector2 GetNavDirection(Vector2 fallback)
    {
        if (_agent.isOnNavMesh && _agent.hasPath && _agent.path.corners.Length > 1)
        {
            Vector2 nextCorner = _agent.path.corners[1];
            return (nextCorner - _rb.position).normalized;
        }
        return fallback;
    }

    private void TryShoot(Vector2 direction)
    {
        if (Time.time < _nextAttackTime || projectilePrefab == null) return;
        _nextAttackTime = Time.time + attackRate;

        float      angle  = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        GameObject bullet = Instantiate(projectilePrefab, _rb.position + direction * 0.6f, Quaternion.Euler(0, 0, angle));
        if (bullet.TryGetComponent(out EnemyProjectile ep))
            ep.Setup(damage, direction);
    }

    void OnDisable()
    {
        if (_agent != null && _agent.isOnNavMesh)
            _agent.ResetPath();
    }
}
