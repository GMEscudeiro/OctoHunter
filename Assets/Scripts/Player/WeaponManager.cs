using UnityEngine;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Data")]
    public WeaponInventory inventory;
    
    [Header("Settings")]
    public float playerDistance = 0.1f;
    public CircleCollider2D playerCollider;

    private List<GameObject> instantiatedWeapons = new List<GameObject>();

    void Start()
    {
        if (playerCollider == null) playerCollider = GetComponent<CircleCollider2D>();
        
        if (inventory != null)
        {
            inventory.OnInventoryChanged += LoadWeaponInventory;
        }

        LoadWeaponInventory();
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= LoadWeaponInventory;
        }
    }

    public void LoadWeaponInventory()
    {
        foreach (GameObject weapon in instantiatedWeapons) Destroy(weapon);
        instantiatedWeapons.Clear();

        if (inventory == null) return;

        foreach (GameObject prefab in inventory.obtainedWeapons)
        {
            GameObject newWeapon = Instantiate(prefab, transform.position, Quaternion.identity, transform);

            if (newWeapon.TryGetComponent(out WeaponBase weaponScript))
            {
                weaponScript.Initialize(this.gameObject);
            }

            instantiatedWeapons.Add(newWeapon);
        }

        OrganizeWeapons();
    }

    private void OrganizeWeapons()
    {
        int amount = instantiatedWeapons.Count;
        if (amount == 0) return;

        float realRadius = playerCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        float finalRadius = realRadius + playerDistance;

        for (int i = 0; i < amount; i++)
        {
            float progress = (float)i / amount;
            float angle = progress * Mathf.PI * 2;

            float x = Mathf.Cos(angle) * finalRadius;
            float y = Mathf.Sin(angle) * finalRadius;

            instantiatedWeapons[i].transform.localPosition = new Vector3(x, y, 0);

            float angleDegrees = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            instantiatedWeapons[i].transform.localRotation = Quaternion.Euler(0, 0, angleDegrees);
        }
    }
}
