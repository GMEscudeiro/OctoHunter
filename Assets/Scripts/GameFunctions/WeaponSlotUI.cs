using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WeaponSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Image iconImage;
    public Image frameImage;
    
    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color selectedColor = Color.yellow;

    private int _index;
    private WeaponBarUI _bar;
    private bool _isSelected;

    public void Setup(int index, Sprite icon, WeaponBarUI bar)
    {
        _index = index;
        _bar = bar;
        
        if (iconImage != null) iconImage.sprite = icon;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
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
