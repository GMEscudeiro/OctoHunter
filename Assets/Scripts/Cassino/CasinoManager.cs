using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

// Coloque este script em um GameObject na cena do Cassino.
// Monte a UI com 3 "slots" de arma e um botão de Reroll e de Sair.
public class CasinoManager : MonoBehaviour
{
    [Header("Data")]
    public List<WeaponShopItem> allWeapons;   // arraste todos os WeaponShopItems aqui
    public WeaponInventory      weaponInventory;
    public WalletData           walletData;

    [Header("Casino Scene Name")]
    public string casinoSceneName = "Casino";

    [Header("UI - Slots (3 itens)")]
    public Image[]           slotIcons;        // 3 imagens de ícone
    public TextMeshProUGUI[] slotPrices;       // 3 textos de preço
    public Button[]          slotBuyButtons;   // 3 botões de compra

    [Header("Detail Card")]
    public WeaponDetailCard detailCard;
    [Header("UI - Geral")]
    public TextMeshProUGUI coinsText;
    public Button          rerollButton;
    public TextMeshProUGUI rerollPriceText;
    public Button          exitButton;

    private const int RerollCost = 5;
    private const int MaxSlots   = 8;

    private WeaponShopItem[] _currentOffer = new WeaponShopItem[3];

    void Start()
    {
        UpdateCoinsUI();
        rerollPriceText.text = $"Trocar ({RerollCost} moedas)";

        rerollButton.onClick.AddListener(Reroll);
        exitButton.onClick.AddListener(Exit);

        RollNewOffers();
    }

    // ── Sorteio ──────────────────────────────────────────────────────
    private void RollNewOffers()
    {
        List<WeaponShopItem> pool = new List<WeaponShopItem>(allWeapons);

        for (int i = 0; i < 3; i++)
        {
            if (pool.Count == 0) break;

            int index = Random.Range(0, pool.Count);
            _currentOffer[i] = pool[index];
            pool.RemoveAt(index);   // sem repetição na mesma oferta
        }

        RefreshSlotUI();
    }

    public void Reroll()
    {
        if (walletData.coins < RerollCost)
        {
            Debug.Log("[Casino] Moedas insuficientes para reroll.");
            return;
        }

        walletData.coins -= RerollCost;
        UpdateCoinsUI();
        RollNewOffers();
    }

    // ── UI ────────────────────────────────────────────────────────────
    private void RefreshSlotUI()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = i;   // captura local para o lambda
            WeaponShopItem item = _currentOffer[i];

            if (item == null)
            {
                slotPrices[i].text = "";
                slotBuyButtons[i].interactable = false;
                continue;
            }

            slotPrices[i].text = $"{item.price} moedas";

            if (slotIcons != null && slotIcons.Length > i && item.icon != null)
                slotIcons[i].sprite = item.icon;

            slotBuyButtons[i].onClick.RemoveAllListeners();
            slotBuyButtons[i].onClick.AddListener(() => detailCard.ShowCard(_currentOffer[index], index));
            slotBuyButtons[i].interactable = true;
        }
    }

    private void UpdateCoinsUI()
    {
        if (coinsText != null)
            coinsText.text = $"Moedas: {walletData.coins}";
    }

    // ── Compra ────────────────────────────────────────────────────────
    public void BuyWeapon(int slotIndex)
    {
        WeaponShopItem item = _currentOffer[slotIndex];
        if (item == null) return;

        // Verifica slots disponíveis
        if (weaponInventory.obtainedWeapons.Count >= MaxSlots)
        {
            Debug.Log("[Casino] Todos os tentáculos estão ocupados!");
            return;
        }

        // Verifica moedas
        if (walletData.coins < item.price)
        {
            Debug.Log("[Casino] Moedas insuficientes.");
            return;
        }

        walletData.coins -= item.price;
        weaponInventory.AddWeapon(item.weaponPrefab);
        UpdateCoinsUI();

        // Remove o slot comprado da oferta
        _currentOffer[slotIndex] = null;
        RefreshSlotUI();

        Debug.Log($"[Casino] Comprou: {item.weaponName}");
    }

    // ── Sair ──────────────────────────────────────────────────────────
    private void Exit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
}
