using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Universal Stats")]
    public int damage = 10;
    public float attackRate = 1.0f;
    public WeaponEffect specialEffect;

    [Header("Sound")]
    public AudioClip attackSound;
    [Range(0f, 1f)] public float soundVolume = 1f;
    [Tooltip("Tempo mínimo entre reproduções para evitar sobreposição excessiva (útil para armas rápidas)")]
    public float soundCooldown = 0.1f;
    [HideInInspector] public bool playSoundEnabled = true;

    protected float nextAttackTime;
    protected GameObject attackerRef;
    private AudioSource _audioSource;
    private float _lastSoundTime;

    public virtual void Initialize(GameObject player)
    {
        attackerRef = player;
        nextAttackTime = Time.time;
        _lastSoundTime = 0f;

        // Cria um AudioSource fixo apenas na arma do primeiro slot
        if (playSoundEnabled && attackSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = attackSound;
            _audioSource.volume = soundVolume;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f; // 2D sound
        }
    }

    protected virtual void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
            
            if (playSoundEnabled)
            {
                if (_audioSource == null) 
                    Debug.LogWarning($"[WeaponBase] {gameObject.name} (Slot 0) tentou tocar som, mas _audioSource é nulo! attackSound={attackSound}");
                else if (DialogueUI.IsCutsceneActive)
                    Debug.Log($"[WeaponBase] {gameObject.name} silenciado por cutscene ativa.");
                else if (Time.time >= _lastSoundTime + soundCooldown)
                {
                    // Usa PlayOneShot para não cortar o som se a arma atirar muito rápido,
                    // mas o soundCooldown impede que dezenas de sons acumulem ao mesmo tempo.
                    _audioSource.PlayOneShot(attackSound, soundVolume);
                    _lastSoundTime = Time.time;
                }
            }

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

