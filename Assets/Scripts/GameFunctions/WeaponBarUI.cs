using UnityEngine;
using System.Collections.Generic;

public class WeaponBarUI : MonoBehaviour
{
    [Header("Data")]
    public WeaponInventory inventory;
    
    [Header("References")]
    public GameObject slotPrefab;
    public Transform container;

    private List<WeaponSlotUI> _instantiatedSlots = new List<WeaponSlotUI>();
    private int _firstSelectedIndex = -1;

    void Start()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshUI;
        }
        RefreshUI();
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        // Clear old slots
        foreach (var slot in _instantiatedSlots) Destroy(slot.gameObject);
        _instantiatedSlots.Clear();
        _firstSelectedIndex = -1;

        if (inventory == null || slotPrefab == null || container == null) return;

        for (int i = 0; i < inventory.obtainedWeapons.Count; i++)
        {
            GameObject prefab = inventory.obtainedWeapons[i];
            GameObject slotObj = Instantiate(slotPrefab, container);
            
            if (slotObj.TryGetComponent(out WeaponSlotUI slotScript))
            {
                // Try to get sprite from the prefab's SpriteRenderer
                Sprite icon = null;
                if (prefab.TryGetComponent(out SpriteRenderer sr))
                {
                    icon = sr.sprite;
                }

                slotScript.Setup(i, icon, this);
                _instantiatedSlots.Add(slotScript);
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        if (_firstSelectedIndex == -1)
        {
            // First selection
            _firstSelectedIndex = index;
            _instantiatedSlots[index].SetSelected(true);
        }
        else if (_firstSelectedIndex == index)
        {
            // Deselect
            _instantiatedSlots[index].SetSelected(false);
            _firstSelectedIndex = -1;
        }
        else
        {
            // Second selection - Perform Swap
            inventory.SwapWeapons(_firstSelectedIndex, index);
            // RefreshUI will be called automatically via the inventory event
        }
    }
}
