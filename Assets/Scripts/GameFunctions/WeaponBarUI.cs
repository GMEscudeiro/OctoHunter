using UnityEngine;
using System.Collections.Generic;

public class WeaponBarUI : MonoBehaviour
{
    [Header("Data")]
    public WeaponInventory inventory;
    
    [Header("References")]
    public GameObject slotPrefab;
    public Transform container;

    [Header("Settings")]
    public int totalSlots = 8;

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

        for (int i = 0; i < totalSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, container);
            
            if (slotObj.TryGetComponent(out WeaponSlotUI slotScript))
            {
                Sprite icon = null;
                
                // If there's a weapon at this index, get its icon
                if (i < inventory.obtainedWeapons.Count)
                {
                    GameObject prefab = inventory.obtainedWeapons[i];
                    if (prefab != null && prefab.TryGetComponent(out SpriteRenderer sr))
                    {
                        icon = sr.sprite;
                    }
                }

                slotScript.Setup(i, icon, this);
                _instantiatedSlots.Add(slotScript);
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        // Only allow selecting slots that actually have weapons
        if (index >= inventory.obtainedWeapons.Count) return;

        if (_firstSelectedIndex == -1)
        {
            _firstSelectedIndex = index;
            _instantiatedSlots[index].SetSelected(true);
        }
        else if (_firstSelectedIndex == index)
        {
            _instantiatedSlots[index].SetSelected(false);
            _firstSelectedIndex = -1;
        }
        else
        {
            inventory.SwapWeapons(_firstSelectedIndex, index);
        }
    }
}
