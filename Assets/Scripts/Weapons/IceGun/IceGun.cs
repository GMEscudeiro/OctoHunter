using UnityEngine;

public class IceGun : WeaponBase
{
    [Header("IceGun Settings")]
    public GameObject projectilePrefab;

    protected override void PerformAttack()
    {
        if (projectilePrefab == null) return;

        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.isIceProjectile = true;
            projectile.Setup(CreateHitData());
        }
    }
}
