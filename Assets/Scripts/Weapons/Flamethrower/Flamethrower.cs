using UnityEngine;

public class Flamethrower : WeaponBase
{
    [Header("Flamethrower Settings")]
    public float range        = 2.5f;   // alcance da chama
    public float coneAngle    = 45f;    // ângulo do cone à frente da arma

    // No Inspector: damage baixo (~5), attackRate rápido (~0.1s) = dano por segundo

    protected override void PerformAttack()
    {
        // Pega todos os colisores dentro do alcance
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range);

        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out Enemy enemy)) continue;

            // Verifica se o inimigo está dentro do cone da arma
            Vector2 dirToEnemy = ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;
            float   angle      = Vector2.Angle(transform.right, dirToEnemy);

            if (angle <= coneAngle / 2f)
            {
                enemy.TakeDamage(CreateHitData());
            }
        }
    }

    // Gizmo para visualizar o alcance no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
