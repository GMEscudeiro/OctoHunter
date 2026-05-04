using UnityEngine;

public class BossAbilityBulletRing : MonoBehaviour
{
    [Header("Settings")]
    public GameObject projectilePrefab;
    public int        bulletCount = 12;    // quantidade de projéteis no anel
    public int        damage      = 1;
    public float      bulletSpeed = 6f;

    public void Activate(Transform origin)
    {
        if (projectilePrefab == null) return;

        float angleStep = 360f / bulletCount;

        Debug.Log("[SnakeBoss] Habilidade: Chuva de Projéteis!");

        for (int i = 0; i < bulletCount; i++)
        {
            float   angle     = i * angleStep;
            Vector2 direction = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad)
            );

            Quaternion rotation  = Quaternion.Euler(0, 0, angle);
            GameObject bullet    = Instantiate(projectilePrefab, origin.position, rotation);

            if (bullet.TryGetComponent(out EnemyProjectile ep))
                ep.Setup(damage, direction);
        }
    }
}
