using System.Collections.Generic;
using UnityEngine;

public class CasinoDialogueManager : MonoBehaviour
{
    public static CasinoDialogueManager Instance { get; private set; }

    [Header("Diálogos Situacionais")]
    [SerializeField] private DialogueData noCoinsDialogue;
    [SerializeField] private DialogueData inventoryFullDialogue;
    [SerializeField] private DialogueData onEnterDialogue;

    private bool _enterDialoguePlayed = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (!_enterDialoguePlayed && onEnterDialogue != null)
        {
            _enterDialoguePlayed = true;
            Play(onEnterDialogue);
        }
    }

    public void PlayNoCoinsDialogue()       => Play(noCoinsDialogue);
    public void PlayInventoryFullDialogue() => Play(inventoryFullDialogue);

    private void Play(DialogueData data)
    {
        if (data == null || GameFlowManager.Instance == null) return;
        GameFlowManager.Instance.PlayDialogueSequence(new List<DialogueData> { data }, null);
    }
}
