using UnityEngine;
using System.Collections;

public class Katana : WeaponBase
{
    [Header("Katana Settings")]
    public float range = 1.5f;
    public float arcAngle = 90f;
    
    [Header("Visuals")]
    public Transform visualTransform; // The sprite/visual part to rotate
    public SpriteRenderer slashRenderer; // Renderer for the slash effect
    public Sprite[] slashSprites; // List of sprites for the slash effect
    public float swingDuration = 0.1f;
    public float swingVisualAngle = 60f;

    private bool _isSwinging;

    protected override void PerformAttack()
    {
        // 1. Visual Effect
        if (!_isSwinging)
        {
            StartCoroutine(SwingRoutine());
        }

        // 2. Randomize Slash Sprite
        if (slashRenderer != null && slashSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, slashSprites.Length);
            slashRenderer.sprite = slashSprites[randomIndex];
            // Flip randomly for more variety
            slashRenderer.flipY = Random.value > 0.5f;
        }

        // 3. Damage Logic
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                // Check if the enemy is within the arc in front of the weapon
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
        if (slashRenderer != null) slashRenderer.enabled = true;
        
        Transform targetTransform = visualTransform != null ? visualTransform : transform;
        Quaternion originalRotation = targetTransform.localRotation;

        // Rotate for the swing
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
        if (slashRenderer != null) slashRenderer.enabled = false;
        _isSwinging = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        
        // Draw the arc
        Vector3 leftDir = Quaternion.Euler(0, 0, -arcAngle / 2f) * transform.right;
        Vector3 rightDir = Quaternion.Euler(0, 0, arcAngle / 2f) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * range);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * range);
    }
}
