using UnityEngine;
using UnityEditor;

public class MagnetSetup
{
    [MenuItem("OctoHunter/Setup Magnet Item")]
    public static void CreateMagnet()
    {
        // 1. Ensure folders exist
        if (!AssetDatabase.IsValidFolder("Assets/WeaponsPrefabs/Magnet"))
            AssetDatabase.CreateFolder("Assets/WeaponsPrefabs", "Magnet");

        // 2. Create Prefab
        string prefabPath = "Assets/WeaponsPrefabs/Magnet/Magnet.prefab";
        GameObject magnetObj = new GameObject("Magnet");
        
        SpriteRenderer sr = magnetObj.AddComponent<SpriteRenderer>();
        Sprite magnetSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/MagnetIcon.png");
        sr.sprite = magnetSprite;
        
        // Add MagnetWeapon script
        MagnetWeapon mw = magnetObj.AddComponent<MagnetWeapon>();
        mw.pullRadius = 5f;
        mw.attackRate = 0.5f; // Pulls every 0.5 seconds
        mw.damage = 0;
        
        // Add WeaponInfo script for shop icon
        WeaponInfo info = magnetObj.AddComponent<WeaponInfo>();
        info.icon = magnetSprite;

        // Save prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(magnetObj, prefabPath);
        GameObject.DestroyImmediate(magnetObj);

        // 3. Create WeaponShopItem ScriptableObject
        string soPath = "Assets/WeaponsPrefabs/Magnet/MagnetShopItem.asset";
        WeaponShopItem shopItem = ScriptableObject.CreateInstance<WeaponShopItem>();
        shopItem.weaponName = "Imã de Moedas";
        shopItem.weaponPrefab = prefab;
        shopItem.price = 10;
        shopItem.description = "Puxa moedas ao seu redor constantemente.";
        shopItem.rarity = WeaponShopItem.Rarity.Raro;

        AssetDatabase.CreateAsset(shopItem, soPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Magnet Setup Complete! Don't forget to add the 'MagnetShopItem' to the 'allWeapons' list in your CasinoManager scene.");
    }

    // ── Mac10 ─────────────────────────────────────────────────────────
    [MenuItem("OctoHunter/Setup Mac10 Item")]
    public static void CreateMac10()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WeaponsPrefabs/Mac10"))
            AssetDatabase.CreateFolder("Assets/WeaponsPrefabs", "Mac10");

        // Reuse pistol's projectile prefab
        GameObject pistolPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/WeaponsPrefabs/Pistol/Pistol.prefab");
        GameObject projectilePrefab = null;
        if (pistolPrefab != null && pistolPrefab.TryGetComponent(out Pistol pistolScript))
        {
            projectilePrefab = pistolScript.projectilePrefab;
        }

        string prefabPath = "Assets/WeaponsPrefabs/Mac10/Mac10.prefab";
        GameObject mac10Obj = new GameObject("Mac10");

        SpriteRenderer sr = mac10Obj.AddComponent<SpriteRenderer>();
        Sprite mac10Sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Mac10Icon.png");
        sr.sprite = mac10Sprite;

        Mac10 mac10 = mac10Obj.AddComponent<Mac10>();
        mac10.damage = 5;         // Dano menor que a pistola
        mac10.attackRate = 0.2f;   // Cadência muito mais alta
        mac10.projectilePrefab = projectilePrefab;

        WeaponInfo info = mac10Obj.AddComponent<WeaponInfo>();
        info.icon = mac10Sprite;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(mac10Obj, prefabPath);
        GameObject.DestroyImmediate(mac10Obj);

        string soPath = "Assets/WeaponsPrefabs/Mac10/Mac10ShopItem.asset";
        WeaponShopItem shopItem = ScriptableObject.CreateInstance<WeaponShopItem>();
        shopItem.weaponName = "Mac 10";
        shopItem.weaponPrefab = prefab;
        shopItem.price = 15;
        shopItem.description = "Submetralhadora rápida com alto rate de fogo e baixo dano.";
        shopItem.rarity = WeaponShopItem.Rarity.Raro;

        AssetDatabase.CreateAsset(shopItem, soPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Mac10 Setup Complete! Add 'Mac10ShopItem' to the CasinoManager allWeapons list.");
    }

    // ── Baseball Bat ──────────────────────────────────────────────────
    [MenuItem("OctoHunter/Setup Baseball Bat Item")]
    public static void CreateBaseballBat()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WeaponsPrefabs/BaseballBat"))
            AssetDatabase.CreateFolder("Assets/WeaponsPrefabs", "BaseballBat");

        string prefabPath = "Assets/WeaponsPrefabs/BaseballBat/BaseballBat.prefab";
        GameObject batObj = new GameObject("BaseballBat");

        SpriteRenderer sr = batObj.AddComponent<SpriteRenderer>();
        Sprite batSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/BaseballBatIcon.png");
        sr.sprite = batSprite;

        BaseballBat bat = batObj.AddComponent<BaseballBat>();
        bat.damage = 10;          // 50HP / 10 = 5 hits para matar um inimigo comum
        bat.attackRate = 0.6f;
        bat.range = 1.0f;
        bat.arcAngle = 120f;

        WeaponInfo info = batObj.AddComponent<WeaponInfo>();
        info.icon = batSprite;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(batObj, prefabPath);
        GameObject.DestroyImmediate(batObj);

        string soPath = "Assets/WeaponsPrefabs/BaseballBat/BaseballBatShopItem.asset";
        WeaponShopItem shopItem = ScriptableObject.CreateInstance<WeaponShopItem>();
        shopItem.weaponName = "Bastão de Baseball";
        shopItem.weaponPrefab = prefab;
        shopItem.price = 8;
        shopItem.description = "Arma corpo-a-corpo com arco amplo. Precisa de 5 hits para matar.";
        shopItem.rarity = WeaponShopItem.Rarity.Comum;

        AssetDatabase.CreateAsset(shopItem, soPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Baseball Bat Setup Complete! Add 'BaseballBatShopItem' to the CasinoManager allWeapons list.");
    }

    // ── Shield ────────────────────────────────────────────────────────
    [MenuItem("OctoHunter/Setup Shield Item")]
    public static void CreateShield()
    {
        if (!AssetDatabase.IsValidFolder("Assets/WeaponsPrefabs/Shield"))
            AssetDatabase.CreateFolder("Assets/WeaponsPrefabs", "Shield");

        string prefabPath = "Assets/WeaponsPrefabs/Shield/Shield.prefab";
        GameObject shieldObj = new GameObject("Shield");

        SpriteRenderer sr = shieldObj.AddComponent<SpriteRenderer>();
        Sprite shieldSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/ShieldIcon.png");
        sr.sprite = shieldSprite;

        ShieldWeapon shield = shieldObj.AddComponent<ShieldWeapon>();
        shield.bonusHits = 1;
        shield.damage = 0;
        shield.attackRate = 999f; // Nunca ataca

        WeaponInfo info = shieldObj.AddComponent<WeaponInfo>();
        info.icon = shieldSprite;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(shieldObj, prefabPath);
        GameObject.DestroyImmediate(shieldObj);

        string soPath = "Assets/WeaponsPrefabs/Shield/ShieldShopItem.asset";
        WeaponShopItem shopItem = ScriptableObject.CreateInstance<WeaponShopItem>();
        shopItem.weaponName = "Escudo";
        shopItem.weaponPrefab = prefab;
        shopItem.price = 12;
        shopItem.description = "Ganha +1 coração extra. Quebra após receber 1 hit.";
        shopItem.rarity = WeaponShopItem.Rarity.Raro;

        AssetDatabase.CreateAsset(shopItem, soPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Shield Setup Complete! Add 'ShieldShopItem' to the CasinoManager allWeapons list.");
    }
}
