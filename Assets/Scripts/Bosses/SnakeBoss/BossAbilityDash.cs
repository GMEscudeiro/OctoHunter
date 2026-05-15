using UnityEngine;
using System.Collections;

public class BossAbilityDash : MonoBehaviour
{
    [Header("Settings")]
    public float dashSpeed    = 20f;
    public float dashDuration = 0.3f;
    public int   dashDamage   = 1;

    private bool _isDashing = false;
    public bool IsDashing => _isDashing;

    public void Activate(Transform playerTransform, Rigidbody2D rb)
    {
        if (!_isDashing)
            StartCoroutine(DashRoutine(playerTransform, rb));
    }

    private IEnumerator DashRoutine(Transform playerTransform, Rigidbody2D rb)
    {
        _isDashing = true;

        // Trava a direção uma única vez — EnemyMovement ficaria sobrescrevendo
        // linearVelocity a cada FixedUpdate, causando a curva; desabilita durante o dash
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        Debug.Log("[SnakeBoss] Habilidade: Investida!");

        Enemy enemy = rb.GetComponent<Enemy>();
        float effectiveDashSpeed = dashSpeed * (enemy != null ? enemy.SpeedMultiplier : 1f);

        EnemyMovement enemyMovement = rb.GetComponent<EnemyMovement>();
        if (enemyMovement != null) enemyMovement.enabled = false;

        rb.linearVelocity = direction * effectiveDashSpeed;
        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = Vector2.zero;

        if (enemyMovement != null) enemyMovement.enabled = true;
        _isDashing = false;
    }
}