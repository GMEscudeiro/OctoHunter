using UnityEngine;
using System.Collections;

// Prefab precisa de: SpriteRenderer (verde/roxo), Collider2D (Is Trigger), este script.
public class VenomPool : MonoBehaviour
{
    [Header("Settings")]
    public int   damagePerTick  = 1;
    public float tickInterval   = 0.5f;   // dano a cada X segundos
    public float lifetime       = 5f;     // duração da poça

    private void Start()
    {
        StartCoroutine(LifetimeRoutine());
    }

    private IEnumerator LifetimeRoutine()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.gameObject.activeInHierarchy) return;
        if (other.TryGetComponent(out PlayerHealth ph) && !ph.IsDead)
            ph.TakeDamage(damagePerTick);
    }
}
