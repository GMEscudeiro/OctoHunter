using System.Collections.Generic;
using UnityEngine;

public class CasinoDialogueManager : MonoBehaviour
{
    public static CasinoDialogueManager Instance { get; private set; }

    [Header("Diálogos Situacionais")]
    [SerializeField] private DialogueData noCoinsDialogue;
    [SerializeField] private DialogueData inventoryFullDialogue;
    [SerializeField] private DialogueData onEnterDialogue;

    [Header("Diálogos de Compra por Raridade")]
    [SerializeField] private DialogueData buyComumDialogue;
    [SerializeField] private DialogueData buyRaroDialogue;
    [SerializeField] private DialogueData buyEpicoDialogue;

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

    public void PlayBuyDialogue(WeaponShopItem.Rarity rarity)
    {
        switch (rarity)
        {
            case WeaponShopItem.Rarity.Comum: Play(buyComumDialogue); break;
            case WeaponShopItem.Rarity.Raro:  Play(buyRaroDialogue);  break;
            case WeaponShopItem.Rarity.Epico: Play(buyEpicoDialogue); break;
        }
    }

    private void Play(DialogueData data)
    {
        if (data == null || GameFlowManager.Instance == null) return;
        GameFlowManager.Instance.PlayDialogueSequence(new List<DialogueData> { data }, null);
    }
}
