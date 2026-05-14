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

        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        Debug.Log("[SnakeBoss] Habilidade: Investida!");

        Enemy enemy = rb.GetComponent<Enemy>();
        float effectiveDashSpeed = dashSpeed * (enemy != null ? enemy.SpeedMultiplier : 1f);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            rb.MovePosition(rb.position + direction * effectiveDashSpeed * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isDashing = false;
    }
}