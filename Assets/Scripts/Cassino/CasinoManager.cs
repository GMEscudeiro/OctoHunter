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

    private void Exit()
    {
        Time.timeScale = 1f;
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogError("[CasinoManager] GameFlowManager não encontrado ao tentar sair!");
            SceneManager.LoadScene("StartMenu");
        }
    }
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
    public Button          sellButton;

    private const int RerollCost = 5;
    private const int MaxSlots   = 8;

    private WeaponShopItem[] _currentOffer = new WeaponShopItem[3];

    void Start()
    {
        UpdateCoinsUI();
        rerollPriceText.text = $"Trocar ({RerollCost} moedas)";

        rerollButton.onClick.AddListener(Reroll);
        exitButton.onClick.AddListener(Exit);
        if (sellButton != null) sellButton.onClick.AddListener(ToggleSellMode);

        RollNewOffers();
    }

    [Header("Probabilidades (Pesos)")]
    public float chanceComum = 60f;
    public float chanceRaro = 30f;
    public float chanceEpico = 10f;

    // ── Sorteio ──────────────────────────────────────────────────────
    private void RollNewOffers()
    {
        List<WeaponShopItem> pool = new List<WeaponShopItem>(allWeapons);

        for (int i = 0; i < 3; i++)
        {
            if (pool.Count == 0) break;

            WeaponShopItem.Rarity selectedRarity = GetRandomRarity();
            List<WeaponShopItem> filteredPool = pool.FindAll(item => item.rarity == selectedRarity);

            if (filteredPool.Count == 0)
            {
                // Fallback caso a raridade sorteada não tenha mais itens disponíveis
                int fallbackIndex = Random.Range(0, pool.Count);
                _currentOffer[i] = pool[fallbackIndex];
                pool.RemoveAt(fallbackIndex);
            }
            else
            {
                int indexInFiltered = Random.Range(0, filteredPool.Count);
                WeaponShopItem chosenItem = filteredPool[indexInFiltered];
                _currentOffer[i] = chosenItem;
                pool.Remove(chosenItem); // Remove para não repetir o mesmo item
            }
        }

        RefreshSlotUI();
    }

    private WeaponShopItem.Rarity GetRandomRarity()
    {
        float totalWeight = chanceComum + chanceRaro + chanceEpico;
        float roll = Random.Range(0f, totalWeight);

        if (roll <= chanceComum)
            return WeaponShopItem.Rarity.Comum;
        else if (roll <= chanceComum + chanceRaro)
            return WeaponShopItem.Rarity.Raro;
        else
            return WeaponShopItem.Rarity.Epico;
    }

    public void Reroll()
    {
        if (walletData.coins < RerollCost)
        {
            Debug.Log("[Casino] Moedas insuficientes para reroll.");
            CasinoDialogueManager.Instance?.PlayNoCoinsDialogue();
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

            if (slotIcons != null && slotIcons.Length > i)
                slotIcons[i].sprite = item.GetIcon();

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
    public bool isSelling  = false;
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
            CasinoDialogueManager.Instance?.PlayNoCoinsDialogue();
            return;
        }

        if (weaponInventory.obtainedWeapons.Count >= MaxSlots)
        {
            Debug.Log("[Casino] Inventário cheio! Clique em uma arma na barra abaixo para substituir.");
            isSwapping = true;
            _pendingSwapSlotIndex = slotIndex;
            CasinoDialogueManager.Instance?.PlayInventoryFullDialogue();

            if (coinsText != null)
                coinsText.text = "Clique em um slot abaixo para trocar!";
            return;
        }

        walletData.coins -= item.price;
        weaponInventory.AddWeapon(item.weaponPrefab);
        UpdateCoinsUI();
        CasinoDialogueManager.Instance?.PlayBuyDialogue(item.rarity);

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

    public void ToggleSellMode()
    {
        if (isSwapping) return; // Não permite vender enquanto está trocando
        isSelling = !isSelling;
        UpdateCoinsUI();

        if (isSelling && coinsText != null)
            coinsText.text = "MODO VENDA: Clique em uma arma para vender!";
    }

    public void SellWeapon(int inventoryIndex)
    {
        if (!isSelling) return;
        if (inventoryIndex < 0 || inventoryIndex >= weaponInventory.obtainedWeapons.Count) return;

        GameObject prefab = weaponInventory.obtainedWeapons[inventoryIndex];
        int sellValue = 0;

        // Procura o preço original
        foreach (var item in allWeapons)
        {
            if (item.weaponPrefab == prefab)
            {
                sellValue = item.price / 2;
                break;
            }
        }

        walletData.coins += sellValue;
        weaponInventory.RemoveWeapon(inventoryIndex);
        UpdateCoinsUI();

        Debug.Log($"[Casino] Vendeu arma por {sellValue} moedas.");
    }

    // Exit() moved to top
}
