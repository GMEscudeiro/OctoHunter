using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI coinsText;

    void OnEnable()
    {
        PlayerHealth.OnLivesChanged  += UpdateLives;
        PlayerWallet.OnCoinsChanged  += UpdateCoins;
    }

    void OnDisable()
    {
        PlayerHealth.OnLivesChanged  -= UpdateLives;
        PlayerWallet.OnCoinsChanged  -= UpdateCoins;
    }

    private void UpdateLives(int lives)
    {
        if (livesText != null)
            livesText.text = $"Vidas: {lives}";
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = $"Moedas: {coins}";
    }
}
