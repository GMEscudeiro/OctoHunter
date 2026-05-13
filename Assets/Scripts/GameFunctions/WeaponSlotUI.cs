using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Image iconImage;
    public Image frameImage;
    
    [Header("Colors (Optional if using Overlay)")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    [Header("Selection Visual")]
    public GameObject selectedOverlay; // Se você preferir ativar uma borda/imagem por cima

    private int _index;
    private WeaponBarUI _bar;
    private bool _isSelected;

    public void Setup(int index, Sprite icon, WeaponBarUI bar)
    {
        _index = index;
        _bar = bar;
        
        if (frameImage == null) frameImage = GetComponent<Image>();

        if (iconImage != null) 
        {
            iconImage.sprite = icon;
            iconImage.enabled = (icon != null); // Hide if empty
        }
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        
        // Se houver um objeto de "Borda Selecionada", ativa/desativa ele
        if (selectedOverlay != null)
        {
            selectedOverlay.SetActive(selected);
        }

        // Continua mudando a cor do frame caso seja útil
        if (frameImage != null)
        {
            frameImage.color = selected ? selectedColor : normalColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _bar.OnSlotClicked(_index);
    }
}
