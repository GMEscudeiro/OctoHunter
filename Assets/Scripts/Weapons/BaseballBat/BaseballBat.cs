using UnityEngine;
using System.Collections;

public class BaseballBat : WeaponBase
{
    [Header("Bat Settings")]
    public float range = 1.0f;
    public float arcAngle = 120f;

    [Header("Visuals")]
    public Transform visualTransform;
    public float swingDuration = 0.1f;
    public float swingVisualAngle = 70f;

    private bool _isSwinging;

    protected override void PerformAttack()
    {
        // 1. Visual swing
        if (!_isSwinging)
        {
            StartCoroutine(SwingRoutine());
        }

        // 2. Damage Logic — hit all enemies within range and arc
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                Vector2 dirToEnemy = (hit.transform.position - transform.position).normalized;
                float angle = Vector2.Angle(transform.right, dirToEnemy);

                if (angle <= arcAngle / 2f)
                {
                    enemy.TakeDamage(CreateHitData());
                }
            }
        }
    }

    private IEnumerator SwingRoutine()
    {
        _isSwinging = true;

        Transform targetTransform = visualTransform != null ? visualTransform : transform;
        Quaternion originalRotation = targetTransform.localRotation;

        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            float t = elapsed / swingDuration;
            float currentAngle = Mathf.Lerp(-swingVisualAngle / 2f, swingVisualAngle / 2f, t);
            targetTransform.localRotation = originalRotation * Quaternion.Euler(0, 0, currentAngle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        targetTransform.localRotation = originalRotation;
        _isSwinging = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);

        Vector3 leftDir = Quaternion.Euler(0, 0, -arcAngle / 2f) * transform.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, arcAngle / 2f) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * range);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * range);
    }
}
