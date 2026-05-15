using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LevelMusicController : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip normalMusic;
    public AudioClip bossMusic;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 0.5f;
    public bool playOnAwake = true;

    [Header("Loop Settings")]
    public bool loopMusic = true;
    [Tooltip("Tempo de pausa (em segundos) antes da música recomeçar.")]
    public float loopCooldown = 2.0f;

    private AudioSource _audioSource;
    private Coroutine _musicCoroutine;
    private AudioClip _currentTrack;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f;
    }

    void OnEnable()
    {
        WaveSpawner.OnBossRoundStarted += SwitchToBossMusic;
    }

    void OnDisable()
    {
        WaveSpawner.OnBossRoundStarted -= SwitchToBossMusic;
    }

    void Start()
    {
        if (playOnAwake && normalMusic != null)
        {
            PlayTrack(normalMusic);
        }
    }

    private void SwitchToBossMusic()
    {
        if (bossMusic != null)
        {
            PlayTrack(bossMusic);
        }
    }

    private void PlayTrack(AudioClip track)
    {
        if (_musicCoroutine != null)
        {
            StopCoroutine(_musicCoroutine);
        }
        
        _currentTrack = track;
        _musicCoroutine = StartCoroutine(MusicRoutine());
    }

    public void StopMusic()
    {
        if (_musicCoroutine != null)
        {
            StopCoroutine(_musicCoroutine);
        }
        _audioSource.Stop();
    }

    private IEnumerator MusicRoutine()
    {
        _audioSource.clip = _currentTrack;
        _audioSource.volume = volume;

        while (true)
        {
            _audioSource.Play();

            // Espera a música terminar
            yield return new WaitForSeconds(_currentTrack.length);

            // Se não for para dar loop, sai do laço e termina
            if (!loopMusic)
            {
                break;
            }

            // Espera o tempo de cooldown antes de tocar novamente
            if (loopCooldown > 0f)
            {
                yield return new WaitForSeconds(loopCooldown);
            }
        }
    }
}
