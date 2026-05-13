using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 50;
    private int _currentHealth;

    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    public static event Action OnEnemyDied;
    public event Action OnDied;
    
    void Start()
    {
        _currentHealth = maxHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    private bool _isDead = false;

    public void TakeDamage(HitData data)
    {
        if (_isDead) return;

        _currentHealth -= data.Damage;

        FlashDamageEffect();

        if (data.Effect != null)
        {
            data.Effect.Apply(this.gameObject);
        }

        if (_currentHealth <= 0)
        {
            _isDead = true;
            Die(data.Attacker);
        }
    }

    private void FlashDamageEffect()
    {
        if (_spriteRenderer == null) return;
        
        _spriteRenderer.color = damageColor;
        Invoke(nameof(ResetColor), 0.1f);
    }

    private void ResetColor()
    {
        _spriteRenderer.color = _originalColor;
    }

    private void Die(GameObject killer)
    {
        if (killer != null && killer.TryGetComponent(out PlayerEvents playerEvents))
        {
            playerEvents.AddScore(10);
        }

        OnEnemyDied?.Invoke();
        OnDied?.Invoke();
        Destroy(gameObject);
    }
}
