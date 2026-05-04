using UnityEngine;

public class Shotgun : WeaponBase
{
    [Header("Shotgun Settings")]
    public GameObject projectilePrefab;
    public int   pelletCount  = 5;
    public float spreadAngle  = 30f;  // ângulo total do leque em graus

    protected override void PerformAttack()
    {
        float startAngle = -spreadAngle / 2f;
        float step       = spreadAngle / (pelletCount - 1);

        for (int i = 0; i < pelletCount; i++)
        {
            float angle = startAngle + step * i;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 0, angle);

            GameObject bullet = Instantiate(projectilePrefab, transform.position, rotation);
            if (bullet.TryGetComponent(out Projectile projectile))
            {
                projectile.Setup(CreateHitData());
            }
        }
    }
}
