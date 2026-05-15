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

    [Header("Sound")]
    public AudioClip damageSound;
    public AudioClip deathSound;
    public AudioClip shieldBreakSound;
    [Range(0f, 1f)] public float soundVolume = 1f;

    public int CurrentLives { get; private set; }
    public bool IsInvincible { get; private set; }
    public int ShieldHits { get; private set; }

    public static event Action<int> OnLivesChanged;  // passa as vidas restantes
    public static event Action        OnPlayerDied;
    public static event Action        OnLastHeart;   // disparado ao chegar em exatamente 1 vida
    public static event Action        OnShieldBroken; // disparado quando o escudo quebra

    void Start()
    {
        CurrentLives = maxLives;
        OnLivesChanged?.Invoke(CurrentLives);

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (_spriteRenderer != null) _originalColor = _spriteRenderer.color;
    }

    public void AddShield(int hits)
    {
        ShieldHits += hits;
        Debug.Log($"[Player] Escudo adicionado! ShieldHits={ShieldHits}");
    }

    public void RemoveShield(int hits)
    {
        ShieldHits = Mathf.Max(0, ShieldHits - hits);
    }

    public void Heal(int amount)
    {
        CurrentLives = Mathf.Min(CurrentLives + amount, maxLives);
        OnLivesChanged?.Invoke(CurrentLives);
    }

    public void SetLives(int value)
    {
        CurrentLives = Mathf.Clamp(value, 0, maxLives);
        OnLivesChanged?.Invoke(CurrentLives);
    }

    public void TakeDamage(int amount = 1)
    {
        if (IsInvincible) return;

        // Shield absorbs the hit completely
        if (ShieldHits > 0)
        {
            ShieldHits--;
            Debug.Log($"[Player] Escudo absorveu o dano! ShieldHits restantes={ShieldHits}");
            if (shieldBreakSound != null)
                AudioSource.PlayClipAtPoint(shieldBreakSound, transform.position, soundVolume);
            FlashDamageEffect();
            StartCoroutine(InvincibilityRoutine());
            OnShieldBroken?.Invoke();
            return;
        }

        CurrentLives -= amount;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives == 1)
            OnLastHeart?.Invoke();

        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position, soundVolume);
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
        Debug.Log("[Player] Morreu!");
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position, soundVolume);
        OnPlayerDied?.Invoke();
        // Desativa o player; a cena de Game Over pode ser carregada por outro script
        gameObject.SetActive(false);
    }
}
