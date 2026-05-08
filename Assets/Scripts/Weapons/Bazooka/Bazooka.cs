using UnityEngine;

public class Bazooka : WeaponBase
{
    [Header("Bazooka Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed  = 5f;    // projétil lento
    public float explosionRadius  = 2.5f;  // raio da explosão
    public int   explosionDamage  = 30;    // dano em área

    protected override void PerformAttack()
    {
        GameObject bullet = Instantiate(projectilePrefab, transform.position, transform.rotation);

        if (bullet.TryGetComponent(out BazookaProjectile bp))
        {
            bp.Setup(CreateHitData(), projectileSpeed, explosionRadius, explosionDamage);
        }
    }
}
