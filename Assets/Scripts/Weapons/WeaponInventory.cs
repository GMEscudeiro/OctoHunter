using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewInventory", menuName = "WeaponInventory")]
public class WeaponInventory : ScriptableObject
{
    public List<GameObject> obtainedWeapons = new List<GameObject>();

    public void ClearInventory() => obtainedWeapons.Clear();
    public void AddWeapon(GameObject prefab) => obtainedWeapons.Add(prefab);
}
