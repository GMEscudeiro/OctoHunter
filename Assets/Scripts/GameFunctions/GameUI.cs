using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("References")]
    public Image[]         heartImages;
    public TextMeshProUGUI coinsText;
    public GameObject      hudContainer;  // pai que agrupa corações + moedas; desativado durante cutscenes

    void OnEnable()
    {
        PlayerHealth.OnLivesChanged  += UpdateLives;
        PlayerWallet.OnCoinsChanged  += UpdateCoins;
        DialogueUI.OnCutsceneStarted += OnCutsceneStarted;
        DialogueUI.OnCutsceneEnded   += OnCutsceneEnded;
    }

    void OnDisable()
    {
        PlayerHealth.OnLivesChanged  -= UpdateLives;
        PlayerWallet.OnCoinsChanged  -= UpdateCoins;
        DialogueUI.OnCutsceneStarted -= OnCutsceneStarted;
        DialogueUI.OnCutsceneEnded   -= OnCutsceneEnded;
    }

    private void OnCutsceneStarted(bool hideHUD) { if (hideHUD && hudContainer != null) hudContainer.SetActive(false); }
    private void OnCutsceneEnded(bool hideHUD)   { if (hideHUD && hudContainer != null) hudContainer.SetActive(true);  }

    private void UpdateLives(int lives)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (heartImages[i] == null) continue;
            heartImages[i].color = i < lives
                ? Color.white
                : new Color(1f, 1f, 1f, 0.2f);
        }
    }

    private void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = $"{coins}";
    }
}
