using UnityEngine;

public class Mac10 : WeaponBase
{
    public GameObject projectilePrefab;

    public override void Initialize(GameObject player)
    {
        base.Initialize(player);
        Debug.Log($"[Mac10] Initialize chamado. attackerRef={attackerRef}, projectilePrefab={projectilePrefab}");
    }

    protected override void PerformAttack()
    {
        if (projectilePrefab == null)
        {
            Debug.LogError("[Mac10] projectilePrefab é NULL! Arraste o prefab do projétil no Inspector.");
            return;
        }

        Debug.Log("[Mac10] Disparando!");
        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);

        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.Setup(CreateHitData());
        }
    }
}
