using UnityEngine;

[CreateAssetMenu(fileName = "FreezeEffect", menuName = "Weapons/Effects/Freeze")]
public class FreezeEffect : WeaponEffect
{
    public float duration       = 3f;
    public float slowMultiplier = 0.3f;

    public override void Apply(GameObject target)
    {
        if (!target.TryGetComponent(out Enemy enemy)) return;

        FreezeStatus status = target.GetComponent<FreezeStatus>();
        if (status == null)
            status = target.AddComponent<FreezeStatus>();

        status.Initialize(enemy, duration, slowMultiplier);
    }
}
