using UnityEngine;
using System.Collections.Generic;

public class LevelDialogueController : MonoBehaviour
{
    [Header("Dialogues")]
    public DialogueData levelStartDialogue;
    public DialogueData fewEnemiesDialogue;
    public DialogueData partsDroppedDialogue;
    public DialogueData lastHeartDialogue;
    public int enemiesThreshold = 2;

    [Header("Settings")]
    public bool triggerStartOnFirstHordeOnly = true;

    private bool _hasTriggeredFewEnemies = false;
    private WaveSpawner _spawner;

    void OnEnable()
    {
        WaveSpawner.OnHordeStarted += HandleHordeStarted;
        Enemy.OnEnemyDied += HandleEnemyDeath;
        WaveSpawner.OnBossPartsDropped += HandlePartsDropped;
        PlayerHealth.OnLastHeart += HandleLastHeart;
    }

    void OnDisable()
    {
        WaveSpawner.OnHordeStarted -= HandleHordeStarted;
        Enemy.OnEnemyDied -= HandleEnemyDeath;
        WaveSpawner.OnBossPartsDropped -= HandlePartsDropped;
        PlayerHealth.OnLastHeart -= HandleLastHeart;
    }

    private void HandleHordeStarted(int round, int horde)
    {
        // Atualiza o spawner a cada horda (necessário pois LDC persiste entre cenas via DM)
        _spawner = FindObjectOfType<WaveSpawner>();

        if (levelStartDialogue == null) return;
        if (triggerStartOnFirstHordeOnly && horde != 1) return;

        StartCoroutine(DelayedStartDialogue(levelStartDialogue));
        _hasTriggeredFewEnemies = false;
    }

    private System.Collections.IEnumerator DelayedStartDialogue(DialogueData data)
    {
        yield return new WaitForEndOfFrame();
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.StartDialogue(data);
    }

    private void HandleEnemyDeath()
    {
        if (fewEnemiesDialogue == null || _hasTriggeredFewEnemies) return;

        if (_spawner != null && _spawner.EnemiesAlive <= enemiesThreshold && _spawner.EnemiesAlive > 0)
        {
            DialogueManager.Instance.StartDialogue(fewEnemiesDialogue);
            _hasTriggeredFewEnemies = true;
        }
    }

    private void HandlePartsDropped()
    {
        if (partsDroppedDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(partsDroppedDialogue);
        }
    }

    private void HandleLastHeart()
    {
        if (lastHeartDialogue != null && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(lastHeartDialogue);
        }
    }
}
