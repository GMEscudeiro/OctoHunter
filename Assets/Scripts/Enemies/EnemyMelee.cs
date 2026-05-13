using UnityEngine;

// Requer um Collider2D com "Is Trigger" marcado no mesmo GameObject (ou filho).
[RequireComponent(typeof(Collider2D))]
public class EnemyMelee : MonoBehaviour
{
    [Header("Stats")]
    public int damage = 1;

    [Header("Attack Rate")]
    public float attackCooldown = 1.0f;   // segundos entre cada hit
    private float _nextAttackTime;

    [Header("Special Effects")]
    public bool appliesParalysis = false;
    public float paralyzeDuration = 2f;
    public bool appliesInfection = false;
    public float infectDuration = 2f;

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDealDamage(other.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        TryDealDamage(collision.gameObject);
    }

    private void TryDealDamage(GameObject target)
    {
        if (Time.time < _nextAttackTime) return;

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth == null) playerHealth = target.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (playerHealth.IsInvincible) return;

            playerHealth.TakeDamage(damage);

            if (appliesParalysis)
            {
                PlayerController controller = target.GetComponent<PlayerController>();
                if (controller == null) controller = target.GetComponentInParent<PlayerController>();
                
                if (controller != null)
                {
                    controller.Paralyze(paralyzeDuration);
                }
            }

            if (appliesInfection)
            {
                playerHealth.Infect(infectDuration);
            }

            _nextAttackTime = Time.time + attackCooldown;
        }
    }
}
