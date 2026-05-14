using UnityEngine;

public class FreezeStatus : MonoBehaviour
{
    private Enemy _enemy;
    private float _endTime;
    private SpriteRenderer _sr;
    private Color _originalColor;

    public void Initialize(Enemy enemy, float duration, float slowMultiplier)
    {
        _enemy   = enemy;
        _endTime = Time.time + duration;

        _enemy.SetSpeedMultiplier(slowMultiplier);

        _sr = GetComponentInChildren<SpriteRenderer>();
        if (_sr != null)
        {
            _originalColor = _sr.color;
            _sr.color = new Color(0.5f, 0.85f, 1f);
        }
    }

    void Update()
    {
        if (Time.time >= _endTime)
            Restore();
    }

    void OnDestroy()
    {
        Restore();
    }

    private void Restore()
    {
        if (_enemy != null) _enemy.ResetSpeedMultiplier();
        if (_sr != null)    _sr.color = _originalColor;
        if (this != null)   Destroy(this);
    }
}
