using UnityEngine;

public class ShotgunProjectile : Projectile
{
    [Header("Shotgun Settings")]
    public float maxRange = 4f;
    private Vector2 _startPosition;

    public override void Setup(HitData data)
    {
        base.Setup(data);
        _startPosition = transform.position;
    }

    protected override void Update()
    {
        base.Update();

        if (Vector2.Distance(_startPosition, transform.position) >= maxRange)
        {
            Destroy(gameObject);
        }
    }
}
