using UnityEngine;

public class FlamethrowerProjectile : Projectile
{
    [Header("Flamethrower Specific")]
    public float scaleSpeed = 2f;
    public float maxScale = 2f;
    public float scatterAmount = 0.1f;

    private Vector3 _direction;

    public override void Setup(HitData data)
    {
        base.Setup(data);
        
        // Add scatter based on the move direction (aim), not the visual rotation
        float randomOffset = Random.Range(-scatterAmount, scatterAmount);
        _direction = Quaternion.Euler(0, 0, randomOffset * 45f) * _moveDir;
    }

    void Update()
    {
        // Custom movement
        transform.position += _direction * speed * Time.deltaTime;

        // Flames usually expand as they travel
        if (transform.localScale.x < maxScale)
        {
            transform.localScale += Vector3.one * scaleSpeed * Time.deltaTime;
        }
    }
}
