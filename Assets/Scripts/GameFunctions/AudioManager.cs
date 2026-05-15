using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Casino Music")]
    public AudioClip casinoMusic;
    [SerializeField] private string casinoSceneName = "CassinoScene";

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume   = 1f;
    [Tooltip("Pausa (em segundos) antes da música recomeçar no loop.")]
    public float loopCooldown = 2f;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private AudioClip   _currentTrack;
    private AudioClip   _normalTrack;
    private AudioClip   _bossTrack;
    private bool        _musicPaused;
    private Coroutine   _musicCoroutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { Instance = null; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _musicSource = gameObject.AddComponent<AudioSource>();
            _sfxSource   = gameObject.AddComponent<AudioSource>();
            foreach (var src in new[] { _musicSource, _sfxSource })
            {
                src.playOnAwake  = false;
                src.spatialBlend = 0f;
                src.loop         = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded        += OnSceneLoaded;
        WaveSpawner.OnBossRoundStarted  += OnBossRoundStarted;
        DialogueUI.OnCutsceneStarted    += OnCutsceneStarted;
        DialogueUI.OnCutsceneEnded      += OnCutsceneEnded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded        -= OnSceneLoaded;
        WaveSpawner.OnBossRoundStarted  -= OnBossRoundStarted;
        DialogueUI.OnCutsceneStarted    -= OnCutsceneStarted;
        DialogueUI.OnCutsceneEnded      -= OnCutsceneEnded;
    }

    // ── Eventos ──────────────────────────────────────────────────────────────

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _normalTrack = null;
        _bossTrack   = null;

        if (scene.name == casinoSceneName)
            PlayMusic(casinoMusic);
        else
            StopMusic(); // LevelMusicController da cena chamará SetLevelMusic
    }

    private void OnBossRoundStarted()
    {
        if (_bossTrack != null) PlayMusic(_bossTrack);
    }

    private void OnCutsceneStarted(bool _) => PauseMusic();
    private void OnCutsceneEnded(bool _)   => ResumeMusic();

    // ── API pública ───────────────────────────────────────────────────────────

    /// <summary>Chamado pelo LevelMusicController de cada cena para registrar os clips do nível.</summary>
    public void SetLevelMusic(AudioClip normal, AudioClip boss)
    {
        _normalTrack = normal;
        _bossTrack   = boss;
        PlayMusic(_normalTrack);
    }

    /// <summary>Toca um SFX em 2D (one-shot, empilhável).</summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || _sfxSource == null) return;
        _sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    // ── Controle de música ────────────────────────────────────────────────────

    private void PlayMusic(AudioClip track)
    {
        if (_musicCoroutine != null) StopCoroutine(_musicCoroutine);
        _musicPaused  = false;
        _currentTrack = track;

        if (track == null) { _musicSource.Stop(); return; }
        _musicCoroutine = StartCoroutine(MusicLoop(track));
    }

    private void StopMusic()
    {
        if (_musicCoroutine != null) StopCoroutine(_musicCoroutine);
        _musicSource.Stop();
        _currentTrack = null;
    }

    private void PauseMusic()
    {
        if (_currentTrack == null) return;
        _musicPaused = true;
        _musicSource.Pause();
    }

    private void ResumeMusic()
    {
        if (!_musicPaused) return;
        _musicPaused = false;
        _musicSource.UnPause();
    }

    private IEnumerator MusicLoop(AudioClip track)
    {
        _musicSource.clip   = track;
        _musicSource.volume = musicVolume;
        _musicSource.Play();

        while (true)
        {
            // Aguarda a faixa terminar; ignora enquanto pausada (isPlaying=false mas _musicPaused=true)
            yield return new WaitUntil(() => !_musicPaused && !_musicSource.isPlaying);

            if (_currentTrack != track) yield break; // outra faixa assumiu

            if (loopCooldown > 0f) yield return new WaitForSeconds(loopCooldown);

            if (_currentTrack != track) yield break;
            _musicSource.Play();
        }
    }
}
