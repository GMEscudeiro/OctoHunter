using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "OctoHunter/DialogueData")]
public class DialogueData : ScriptableObject
{
    public List<DialogueEntry> entries;
    
    public enum DialogueStyle { Default, Casino }
    
    [Header("Settings")]
    public DialogueStyle style = DialogueStyle.Default;
    public bool isCutscene = false;
    [Tooltip("Se true, sorteia uma entry aleatória em vez de reproduzir todas em sequência")]
    public bool randomEntry = false;
    
    [Tooltip("Time each line stays on screen if it's an ambient dialogue")]
    public float ambientDisplayDuration = 3f;
    
    [Tooltip("Speed of the typewriter effect")]
    public float typingSpeed = 0.05f;

    [Tooltip("Se false, não esconde a HUD ao exibir (use em tutoriais e diálogos in-game)")]
    public bool hideHUD = true;
}
