using UnityEngine;

// Registra os clips de música do nível no AudioManager.
// Todo o controle de reprodução (boss, cutscene, loop) fica no AudioManager.
public class LevelMusicController : MonoBehaviour
{
    [Header("Music Tracks")]
    public AudioClip normalMusic;
    public AudioClip bossMusic;

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetLevelMusic(normalMusic, bossMusic);
    }
}
