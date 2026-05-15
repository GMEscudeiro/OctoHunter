using UnityEngine;

public class MagnetWeapon : WeaponBase
{
    [Header("Magnet Settings")]
    public float pullRadius = 5f;

    public override void Initialize(GameObject player)
    {
        base.Initialize(player);
    }

    protected override void PerformAttack()
    {
        // Fallback: se Initialize() não foi chamado, usa o root do transform
        Transform center = (attackerRef != null) ? attackerRef.transform : transform.root;

        // Busca todas as moedas na cena e puxa as que estão dentro do raio
        CoinPickup[] allCoins = FindObjectsByType<CoinPickup>(FindObjectsSortMode.None);

        foreach (var coin in allCoins)
        {
            if (coin == null) continue;

            float dist = Vector2.Distance(center.position, coin.transform.position);
            if (dist <= pullRadius)
            {
                coin.StartPull(center);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.35f);

        if (attackerRef != null)
        {
            Gizmos.DrawWireSphere(attackerRef.transform.position, pullRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, pullRadius);
        }
    }
}
