using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Coloque este script em um GameObject na cena do Cassino.
// O card deve ser um painel com: imagem da arma, texto de raridade, nome, descrição e botão de compra.
public class WeaponDetailCard : MonoBehaviour
{
    [Header("UI References")]
    public GameObject     cardPanel;        // painel inteiro do card (ativa/desativa)
    public Image          weaponIcon;       // imagem grande da arma
    public TextMeshProUGUI rarityText;      // texto de raridade (Ex: "Épico")
    public TextMeshProUGUI weaponNameText;  // nome da arma
    public TextMeshProUGUI descriptionText; // descrição curta
    public Button         buyButton;        // botão de comprar
    [Header("Rarity Images")]
    public Image cardPanelImage;     // Image do painel principal - Épico
    public Image rarityBadgeImage;   // Image do badge - Épico
    public Image cardPanelComum;     // Image do painel principal - Comum
    public Image rarityBadgeComum;   // Image do badge - Comum
    public Image cardPanelRaro;      // Image do painel principal - Raro
    public Image rarityBadgeRaro;    // Image do badge - Raro

    private WeaponShopItem _currentItem;
    private int            _currentSlotIndex;
    private CasinoManager  _casinoManager;

    void Start()
    {
        _casinoManager = FindObjectOfType<CasinoManager>();
        cardPanel.SetActive(false);

        buyButton.onClick.AddListener(OnBuyClicked);
    }

    // Chamado pelo CasinoManager ao clicar em um slot
    public void ShowCard(WeaponShopItem item, int slotIndex)
    {
        if (item == null)
        {
            HideCard();
            return;
        }

        _currentItem      = item;
        _currentSlotIndex = slotIndex;

        // Preenche os dados
        weaponNameText.text  = item.weaponName;
        descriptionText.text = item.description;

        if (item.icon != null)
            weaponIcon.sprite = item.icon;

        // Raridade
        switch (item.rarity)
        {
            case WeaponShopItem.Rarity.Comum:
                rarityText.text = "Comum";
                cardPanelComum.gameObject.SetActive(true);
                cardPanelRaro.gameObject.SetActive(false);
                cardPanelImage.gameObject.SetActive(false);
                rarityBadgeComum.gameObject.SetActive(true);
                rarityBadgeRaro.gameObject.SetActive(false);
                rarityBadgeImage.gameObject.SetActive(false);
                break;
            case WeaponShopItem.Rarity.Raro:
                rarityText.text = "Raro";
                cardPanelComum.gameObject.SetActive(false);
                cardPanelRaro.gameObject.SetActive(true);
                cardPanelImage.gameObject.SetActive(false);
                rarityBadgeComum.gameObject.SetActive(false);
                rarityBadgeRaro.gameObject.SetActive(true);
                rarityBadgeImage.gameObject.SetActive(false);
                break;
            case WeaponShopItem.Rarity.Epico:
                rarityText.text = "Épico";
                cardPanelComum.gameObject.SetActive(false);
                cardPanelRaro.gameObject.SetActive(false);
                cardPanelImage.gameObject.SetActive(true);
                rarityBadgeComum.gameObject.SetActive(false);
                rarityBadgeRaro.gameObject.SetActive(false);
                rarityBadgeImage.gameObject.SetActive(true);
                break;
        }

        cardPanel.SetActive(true);
    }

    public void HideCard()
    {
        _currentItem = null;
        cardPanel.SetActive(false);
    }

    private void OnBuyClicked()
    {
        if (_currentItem == null) return;
        _casinoManager.BuyWeapon(_currentSlotIndex);
        HideCard();
    }
}
