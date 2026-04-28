using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Universal Stats")]
    public int damage = 10;
    public float attackRate = 1.0f;
    public WeaponEffect specialEffect;

    protected float nextAttackTime;
    protected GameObject attackerRef;

    public virtual void Initialize(GameObject player)
    {
        attackerRef = player;
        nextAttackTime = Time.time;
    }

    protected virtual void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            nextAttackTime = Time.time + attackRate;
        }
    }

    protected abstract void PerformAttack();

    protected HitData CreateHitData()
    {
        return new HitData
        {
            Damage = damage,
            Attacker = attackerRef,
            Effect = specialEffect
        };
    }
}
