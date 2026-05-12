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
        if (Time.time < _nextAttackTime) return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (playerHealth.IsInvincible) return;

            playerHealth.TakeDamage(damage);

            if (appliesParalysis)
            {
                PlayerController controller = other.GetComponent<PlayerController>();
                if (controller == null) controller = other.GetComponentInParent<PlayerController>();
                
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
