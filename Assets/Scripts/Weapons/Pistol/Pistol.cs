using UnityEngine;

public class Pistol : WeaponBase 
{
    public GameObject projectilePrefab;

    protected override void PerformAttack()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);
        
        if (bullet.TryGetComponent(out Projectile projectile))
        {
            projectile.Setup(CreateHitData());
        }
    }
}
