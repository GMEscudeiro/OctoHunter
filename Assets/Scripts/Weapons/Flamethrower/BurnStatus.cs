using UnityEngine;

public class BurnStatus : MonoBehaviour
{
    private int _damagePerTick;
    private float _duration;
    private float _tickInterval;
    private float _endTime;
    private float _nextTickTime;
    private Enemy _enemy;
    private GameObject _attacker;

    public void Initialize(int damage, float duration, float interval, GameObject attacker)
    {
        _damagePerTick = damage;
        _duration = duration;
        _tickInterval = interval;
        _endTime = Time.time + duration;
        _nextTickTime = Time.time + interval;
        _enemy = GetComponent<Enemy>();
        _attacker = attacker;
    }

    void Update()
    {
        if (Time.time >= _endTime)
        {
            Destroy(this);
            return;
        }

        if (Time.time >= _nextTickTime)
        {
            if (_enemy != null)
            {
                // We pass null for effect to avoid infinite recursion/re-application
                _enemy.TakeDamage(new HitData 
                { 
                    Damage = _damagePerTick, 
                    Attacker = _attacker, 
                    Effect = null 
                });
            }
            _nextTickTime = Time.time + _tickInterval;
        }
    }
}
