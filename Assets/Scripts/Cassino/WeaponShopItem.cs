using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponShopItem", menuName = "OctoHunter/WeaponShopItem")]
public class WeaponShopItem : ScriptableObject
{
    public string     weaponName;
    public GameObject weaponPrefab;
    public int        price;
    public Sprite     icon;          // ícone exibido no cassino (pode ser null por enquanto)
}
