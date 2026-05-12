using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewInventory", menuName = "WeaponInventory")]
public class WeaponInventory : ScriptableObject
{
    public List<GameObject> obtainedWeapons = new List<GameObject>();
    public System.Action OnInventoryChanged;

    public void ClearInventory()
    {
        obtainedWeapons.Clear();
        OnInventoryChanged?.Invoke();
    }

    public void AddWeapon(GameObject prefab)
    {
        obtainedWeapons.Add(prefab);
        OnInventoryChanged?.Invoke();
    }

    public void RemoveWeapon(int index)
    {
        if (index < 0 || index >= obtainedWeapons.Count) return;
        obtainedWeapons.RemoveAt(index);
        OnInventoryChanged?.Invoke();
    }

    public void SwapWeapons(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= obtainedWeapons.Count) return;
        if (indexB < 0 || indexB >= obtainedWeapons.Count) return;

        GameObject temp = obtainedWeapons[indexA];
        obtainedWeapons[indexA] = obtainedWeapons[indexB];
        obtainedWeapons[indexB] = temp;

        OnInventoryChanged?.Invoke();
    }
}
