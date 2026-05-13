using UnityEngine;

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

    private Rigidbody2D _rb;
    private Transform   _playerTransform;
    private float       _retreatTimer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;
    }

    void FixedUpdate()
    {
        if (_playerTransform == null) return;

        switch (CurrentState)
        {
            case BossState.Chasing:
                HandleChasing();
                break;
            case BossState.Retreating:
                HandleRetreating();
                break;
            case BossState.Idle:
                break;
        }

        FacePlayer();
    }

    private void HandleChasing()
    {
        Vector2 toPlayer      = (Vector2)_playerTransform.position - _rb.position;
        float   distance      = toPlayer.magnitude;
        Vector2 direction     = toPlayer.normalized;
        Vector2 wallAvoidance = GetWallAvoidance();

        Vector2 moveDir;

        if (distance > preferredRange + reachTolerance)
            moveDir = (direction + wallAvoidance).normalized;
        else if (distance < preferredRange - reachTolerance)
            moveDir = (-direction + wallAvoidance).normalized * 0.5f;
        else
            moveDir = wallAvoidance;

        _rb.MovePosition(_rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    private void HandleRetreating()
    {
        _retreatTimer -= Time.fixedDeltaTime;

        Vector2 awayFromPlayer = (_rb.position - (Vector2)_playerTransform.position).normalized;
        _rb.MovePosition(_rb.position + awayFromPlayer * retreatSpeed * Time.fixedDeltaTime);

        if (_retreatTimer <= 0f)
            SetState(BossState.Chasing);
    }

    private Vector2 GetWallAvoidance()
    {
        Vector2 avoidance     = Vector2.zero;
        float   checkDistance = 2f;

        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (Vector2 dir in directions)
        {
            RaycastHit2D hit = Physics2D.Raycast(_rb.position, dir, checkDistance);
            if (hit.collider != null && hit.collider.CompareTag("Wall"))
                avoidance -= dir * (1f - hit.distance / checkDistance);
        }

        return avoidance;
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
    }
}