using UnityEngine;

[CreateAssetMenu(fileName = "BurnEffect", menuName = "Weapons/Effects/Burn")]
public class BurnEffect : WeaponEffect
{
    public int damagePerTick = 2;
    public float duration = 3f;
    public float tickInterval = 0.5f;

    public override void Apply(GameObject target)
    {
        if (target.TryGetComponent(out Enemy enemy))
        {
            BurnStatus status = target.GetComponent<BurnStatus>();
            if (status == null)
            {
                status = target.AddComponent<BurnStatus>();
            }
            
            // Re-initialize to refresh duration
            status.Initialize(damagePerTick, duration, tickInterval, null);
        }
    }
}
