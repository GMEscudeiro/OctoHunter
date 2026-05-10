using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class SpiderBoss : MonoBehaviour
{
    [Header("References")]
    public BossAbilityDash dashAbility; // Reuse the dash ability
    
    [Header("Attack Pattern")]
    public float timeBetweenAttacks = 3f;
    private BossMovement _movement;
    private Transform _playerTransform;
    private bool _isDead = false;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;

        GetComponent<Enemy>().OnDied += OnBossDied;
        _movement = GetComponent<BossMovement>();
        
        // Setup Paralyze
        EnemyMelee melee = GetComponent<EnemyMelee>();
        if (melee != null)
        {
            melee.appliesParalysis = true;
            melee.paralyzeDuration = 2f;
        }

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);

        while (!_isDead)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);
            if (_isDead) yield break;

            if (Random.value > 0.5f)
                StartCoroutine(DashAndResume());
            else
                StartCoroutine(JumpAttack());
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

    private void OnBossDied()
    {
        _isDead = true;
        Debug.Log("[SpiderBoss] Boss derrotado!");
    }
}
