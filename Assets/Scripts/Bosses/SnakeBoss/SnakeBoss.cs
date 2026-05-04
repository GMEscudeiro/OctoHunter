using UnityEngine;
using System.Collections;

// Coloque este script no prefab do Boss Cobra.
// Requer: Enemy.cs, Rigidbody2D, Collider2D
// Arraste os scripts de habilidade no Inspector.
[RequireComponent(typeof(Enemy))]
[RequireComponent(typeof(Rigidbody2D))]
public class SnakeBoss : MonoBehaviour
{
    [Header("References")]
    public BossAbilityVenomPool  venomAbility;
    public BossAbilityDash       dashAbility;
    public BossAbilityBulletRing bulletRingAbility;

    [Header("Attack Pattern")]
    public float timeBetweenAttacks = 3f;   // segundos entre cada habilidade
    private BossMovement _movement;
    private Transform _playerTransform;
    private int       _attackIndex = 0;
    private bool      _isDead      = false;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) _playerTransform = p.transform;

        GetComponent<Enemy>().OnDied += OnBossDied;
        _movement = GetComponent<BossMovement>();
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        yield return new WaitForSeconds(2f);   // pausa inicial antes de atacar

        while (!_isDead)
        {
            yield return new WaitForSeconds(timeBetweenAttacks);
            if (_isDead) yield break;

            UseNextAbility();
        }
    }

    private void UseNextAbility()
    {
        if (_playerTransform == null) return;

        switch (_attackIndex % 3)
        {
            case 0:
                // Venom: spawna poças e recua
                venomAbility?.Activate(_playerTransform);
                _movement?.SetState(BossMovement.BossState.Retreating);
                break;
            case 1:
                StartCoroutine(DashAndResume());
                break;
            case 2:
                // BulletRing: para no lugar para disparar
                _movement?.SetState(BossMovement.BossState.Idle);
                bulletRingAbility?.Activate(transform);
                _movement?.SetState(BossMovement.BossState.Chasing);
                break;
        }

        _attackIndex++;
    }

    private IEnumerator DashAndResume()
    {
        _movement?.SetState(BossMovement.BossState.Idle);
        dashAbility?.Activate(_playerTransform, GetComponent<Rigidbody2D>());
        // Aguarda o dash terminar antes de retomar o movimento
        yield return new WaitUntil(() => !dashAbility.IsDashing);
        _movement?.SetState(BossMovement.BossState.Chasing);
    }

    private void OnBossDied()
    {
        _isDead = true;
        Debug.Log("[SnakeBoss] Boss derrotado!");
        // Aqui futuramente: dropar peça da nave
    }
}
