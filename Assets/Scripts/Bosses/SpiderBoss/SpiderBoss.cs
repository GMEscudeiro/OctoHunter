using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class SpiderBoss : MonoBehaviour
{
    [Header("References")]
    public BossAbilityDash dashAbility;
    public GameObject paralysisProjectilePrefab;

    [Header("Attack Pattern")]
    public float timeBetweenAttacks = 3f;
    private BossMovement _movement;
    private Transform _playerTransform;
    private bool _isDead = false;
    private Rigidbody2D _rb;

    [Header("Coin Drop")]
    public GameObject coinPrefab;
    public int        coinAmount    = 10;
    public float      scatterRadius = 1.5f;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;

        GetComponent<Enemy>().OnDied += OnBossDied;
        _movement = GetComponent<BossMovement>();

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);

        while (!_isDead)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);
            if (_isDead) yield break;

            float r = Random.value;
            if (r < 0.34f)
                StartCoroutine(DashAndResume());
            else if (r < 0.67f)
                StartCoroutine(JumpAttack());
            else
                ShootParalysisProjectile();
        }
    }

    private IEnumerator DashAndResume()
    {
        _movement?.SetState(BossMovement.BossState.Idle);
        if (dashAbility != null && _playerTransform != null)
        {
            dashAbility.Activate(_playerTransform, _rb);
            yield return new WaitUntil(() => !dashAbility.IsDashing);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }
        _movement?.SetState(BossMovement.BossState.Chasing);
    }

    private IEnumerator JumpAttack()
    {
        if (_playerTransform == null) yield break;

        _movement?.SetState(BossMovement.BossState.Idle);
        
        Vector3 startPos = transform.position;
        Vector3 targetPos = _playerTransform.position;
        float duration = 1.0f;
        float elapsed = 0f;

        // "Jump" visual - scale up and down
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Parabola height
            float height = Mathf.Sin(t * Mathf.PI) * 2f; 
            
            transform.position = Vector3.Lerp(startPos, targetPos, t) + new Vector3(0, height, 0);
            
            yield return null;
        }

        transform.position = targetPos;
        _movement?.SetState(BossMovement.BossState.Chasing);
    }

    private void ShootParalysisProjectile()
    {
        if (_playerTransform == null || paralysisProjectilePrefab == null) return;

        Vector2 direction = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
        float   angle     = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector2 spawnPos  = (Vector2)transform.position + direction * 0.6f;

        GameObject bullet = Instantiate(paralysisProjectilePrefab, spawnPos, Quaternion.Euler(0, 0, angle));
        if (bullet.TryGetComponent(out EnemyProjectile ep))
        {
            ep.appliesParalysis = true;
            ep.paralyzeDuration = 2f;
            ep.Setup(0, direction);
        }
    }

    private void OnBossDied()
    {
        _isDead = true;
        DropCoins();
        Debug.Log("[SpiderBoss] Boss derrotado!");
    }

    private void DropCoins()
    {
        if (coinPrefab == null) return;
        for (int i = 0; i < coinAmount; i++)
        {
            Vector2 offset   = Random.insideUnitCircle * scatterRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset.x, offset.y, 0);
            Instantiate(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}
