using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed      = 2.5f;
    public float preferredRange = 5f;
    public float reachTolerance = 0.5f;

    [Header("Retreat (após Venom)")]
    public float retreatSpeed    = 4f;
    public float retreatDuration = 1.2f;

    [Header("Visual Correction")]
    public float rotationOffset = 90f;

    public enum BossState { Chasing, Retreating, Idle }
    public BossState CurrentState { get; private set; } = BossState.Chasing;

    private Rigidbody2D  _rb;
    private NavMeshAgent _agent;
    private Transform    _playerTransform;
    private float        _retreatTimer;
    private Enemy        _enemy;

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

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;
    }

    void Update()
    {
        if (_playerTransform == null || !_agent.isOnNavMesh) return;

        _agent.nextPosition = transform.position;

        switch (CurrentState)
        {
            case BossState.Chasing:
                UpdateChasingDestination();
                break;
            case BossState.Retreating:
                Vector2 away = (_rb.position - (Vector2)_playerTransform.position).normalized;
                _agent.SetDestination(_rb.position + away * 5f);
                break;
            case BossState.Idle:
                _agent.ResetPath();
                break;
        }
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        switch (CurrentState)
        {
            case BossState.Chasing:    HandleChasing();    break;
            case BossState.Retreating: HandleRetreating(); break;
        }

        FacePlayer();
    }

    private void UpdateChasingDestination()
    {
        Vector2 toPlayer = (Vector2)_playerTransform.position - _rb.position;
        float   distance = toPlayer.magnitude;

        if (distance > preferredRange + reachTolerance)
            _agent.SetDestination(_playerTransform.position);
        else if (distance < preferredRange - reachTolerance)
            _agent.SetDestination(_rb.position + (-toPlayer.normalized * preferredRange));
        else
            _agent.ResetPath();
    }

    private void HandleChasing()
    {
        Vector2 toPlayer = (Vector2)_playerTransform.position - _rb.position;
        float   distance = toPlayer.magnitude;
        float   speed    = moveSpeed * (_enemy != null ? _enemy.SpeedMultiplier : 1f);

        if (Mathf.Abs(distance - preferredRange) <= reachTolerance) return;

        Vector2 moveDir = GetNavDirection(distance > preferredRange ? toPlayer : -toPlayer);
        float   s       = distance < preferredRange ? speed * 0.5f : speed;
        _rb.MovePosition(_rb.position + moveDir * s * Time.fixedDeltaTime);
    }

    private void HandleRetreating()
    {
        _retreatTimer -= Time.fixedDeltaTime;

        float   speed = retreatSpeed * (_enemy != null ? _enemy.SpeedMultiplier : 1f);
        Vector2 away  = (_rb.position - (Vector2)_playerTransform.position).normalized;
        Vector2 dir   = GetNavDirection(away);
        _rb.MovePosition(_rb.position + dir * speed * Time.fixedDeltaTime);

        if (_retreatTimer <= 0f) SetState(BossState.Chasing);
    }

    private Vector2 GetNavDirection(Vector2 fallback)
    {
        if (_agent.isOnNavMesh && _agent.hasPath && _agent.path.corners.Length > 1)
        {
            Vector2 nextCorner = _agent.path.corners[1];
            return (nextCorner - _rb.position).normalized;
        }
        return fallback.normalized;
    }

    private void FacePlayer()
    {
        Vector2 direction = (Vector2)_playerTransform.position - _rb.position;
        float   angle     = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rb.rotation      = angle + rotationOffset;
    }

    public void SetState(BossState state, float duration = 0f)
    {
        CurrentState = state;

        if (state == BossState.Retreating)
            _retreatTimer = duration > 0f ? duration : retreatDuration;

        if (state == BossState.Idle && _agent.isOnNavMesh)
            _agent.ResetPath();
    }
}
