using UnityEngine;

public abstract class WeaponEffect : ScriptableObject
{
    public abstract void Apply(GameObject target);
}
