using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponShopItem", menuName = "OctoHunter/WeaponShopItem")]
public class WeaponShopItem : ScriptableObject
{
    public string     weaponName;
    public GameObject weaponPrefab;
    public int        price;
    public Sprite     icon;     
    [Header("Details")]
    public string description;

    public enum Rarity { Comum, Raro, Epico }
    public Rarity rarity; 
}
