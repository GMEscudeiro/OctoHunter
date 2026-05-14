using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponShopItem", menuName = "OctoHunter/WeaponShopItem")]
public class WeaponShopItem : ScriptableObject
{
    public string     weaponName;
    public GameObject weaponPrefab;
    public int        price;
    [Header("Details")]
    public string description;

    public Sprite GetIcon()
    {
        if (weaponPrefab != null && weaponPrefab.TryGetComponent(out WeaponInfo info))
            return info.icon;
        return null;
    }

    public enum Rarity { Comum, Raro, Epico }
    public Rarity rarity; 
}
