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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (Time.time < _nextAttackTime) return;

        if (other.TryGetComponent(out PlayerHealth playerHealth))
        {
            playerHealth.TakeDamage(damage);
            _nextAttackTime = Time.time + attackCooldown;
        }
    }
}
