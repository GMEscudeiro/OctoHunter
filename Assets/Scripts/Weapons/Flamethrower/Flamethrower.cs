using UnityEngine;

public class Flamethrower : WeaponBase
{
    [Header("Flamethrower Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    

    protected override void PerformAttack()
    {
        if (projectilePrefab == null) return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        
        GameObject projObj = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
        if (projObj.TryGetComponent(out Projectile projectile))
        {
            projectile.Setup(CreateHitData());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.4f);
        if (firePoint != null)
            Gizmos.DrawWireSphere(firePoint.position, 0.5f);
        else
            Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
