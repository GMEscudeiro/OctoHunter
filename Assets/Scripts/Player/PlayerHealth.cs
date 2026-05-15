using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxLives = 3;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;

    [Header("Visual Feedback")]
    public Color damageColor = Color.red;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor = Color.white;

    public int CurrentLives { get; private set; }
    public bool IsInvincible { get; private set; }
    public bool IsDead       { get; private set; }

    public static event Action<int> OnLivesChanged;
    public static event Action      OnPlayerDied;
    public static event Action      OnLastHeart;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        OnLivesChanged = null;
        OnPlayerDied   = null;
        OnLastHeart    = null;
    }

    void Start()
    {
        CurrentLives = maxLives;
        OnLivesChanged?.Invoke(CurrentLives);

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    public void TakeDamage(int amount = 1)
    {
        if (IsInvincible || IsDead) return;

        CurrentLives -= amount;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives == 1)
            OnLastHeart?.Invoke();

        FlashDamageEffect();

        if (CurrentLives <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityRoutine());
    }

    private void FlashDamageEffect()
    {
        if (_spriteRenderer == null) return;
        
        _spriteRenderer.color = damageColor;
        Invoke(nameof(ResetColor), 0.1f);
    }

    private void ResetColor()
    {
        if (_spriteRenderer != null) _spriteRenderer.color = _originalColor;
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        IsInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        IsInvincible = false;
    }

    public void Infect(float duration)
    {
        StartCoroutine(InfectRoutine(duration));
    }

    private System.Collections.IEnumerator InfectRoutine(float duration)
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) yield break;

        float elapsed = 0f;
        Color originalColor = sr.color;
        
        while (elapsed < duration)
        {
            sr.color = new Color(0.5f, 1f, 0.5f, 0.5f); // Greenish and semi-transparent
            yield return new WaitForSeconds(0.1f);
            sr.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.2f;
        }

        sr.color = originalColor;
    }

    private void Die()
    {
        IsDead = true;
        OnPlayerDied?.Invoke();
        gameObject.SetActive(false);
    }
}
