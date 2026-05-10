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
    public LevelData            levelData;    // para saber qual cena retornar

    [Header("Cenas de jogo (por round)")]
    [Tooltip("Cena do deserto (rounds 1-3)")]
    public string desertSceneName  = "SampleScene";
    [Tooltip("Cena das aranhas (rounds 4+)")]
    public string spiderSceneName  = "SpiderScene";

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

    public static CasinoManager instance;
    public bool isSwapping = false;
    private int _pendingSwapSlotIndex = -1;

    void Awake()
    {
        instance = this;
    }

    // ── Compra ────────────────────────────────────────────────────────
    public void BuyWeapon(int slotIndex)
    {
        WeaponShopItem item = _currentOffer[slotIndex];
        if (item == null) return;

        if (walletData.coins < item.price)
        {
            Debug.Log("[Casino] Moedas insuficientes.");
            return;
        }

        if (weaponInventory.obtainedWeapons.Count >= MaxSlots)
        {
            Debug.Log("[Casino] Inventário cheio! Clique em uma arma na barra abaixo para substituir.");
            isSwapping = true;
            _pendingSwapSlotIndex = slotIndex;
            
            if (coinsText != null)
                coinsText.text = "Clique em um slot abaixo para trocar!";
            return;
        }

        walletData.coins -= item.price;
        weaponInventory.AddWeapon(item.weaponPrefab);
        UpdateCoinsUI();

        _currentOffer[slotIndex] = null;
        RefreshSlotUI();

        Debug.Log($"[Casino] Comprou: {item.weaponName}");
    }

    public void ConfirmSwap(int inventoryIndex)
    {
        if (!isSwapping || _pendingSwapSlotIndex < 0) return;

        WeaponShopItem item = _currentOffer[_pendingSwapSlotIndex];
        walletData.coins -= item.price;
        
        weaponInventory.obtainedWeapons[inventoryIndex] = item.weaponPrefab;
        weaponInventory.OnInventoryChanged?.Invoke();

        UpdateCoinsUI();
        _currentOffer[_pendingSwapSlotIndex] = null;
        RefreshSlotUI();
        isSwapping = false;
        _pendingSwapSlotIndex = -1;
    }

    // ── Sair: decide qual cena carregar pelo round atual ──────────────
    private void Exit()
    {
        Time.timeScale = 1f;

        int round = (levelData != null) ? levelData.currentRound : 1;

        if (round == 2)
        {
            Debug.Log($"[Casino] Round {round} → voltando para {desertSceneName} (Boss Cobra)");
            SceneManager.LoadScene(desertSceneName);
        }
        else if (round == 4)
        {
            Debug.Log($"[Casino] Round {round} → voltando para {spiderSceneName} (Boss Aranha)");
            SceneManager.LoadScene(spiderSceneName);
        }
        else if (round >= 6)
        {
            Debug.Log($"[Casino] Round {round} → voltando para ScorpionScene (Boss Escorpiao)");
            SceneManager.LoadScene("ScorpionScene");
        }
        else
        {
            SceneManager.LoadScene(desertSceneName);
        }
    }
}
