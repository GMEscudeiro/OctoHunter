using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxLives = 3;

    [Header("Invincibility")]
    public float invincibilityDuration = 1.5f;

    public int CurrentLives { get; private set; }
    public bool IsInvincible { get; private set; }

    public static event Action<int> OnLivesChanged;  // passa as vidas restantes
    public static event Action        OnPlayerDied;

    void Start()
    {
        CurrentLives = maxLives;
        OnLivesChanged?.Invoke(CurrentLives);
    }

    public void TakeDamage(int amount = 1)
    {
        if (IsInvincible) return;

        CurrentLives -= amount;
        OnLivesChanged?.Invoke(CurrentLives);

        if (CurrentLives <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvincibilityRoutine());
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
        OnPlayerDied?.Invoke();
        // Desativa o player; a cena de Game Over pode ser carregada por outro script
        gameObject.SetActive(false);
    }
}
