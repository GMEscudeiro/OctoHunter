using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpeciesData", menuName = "OctoHunter/Species Data")]
public class SpeciesData : ScriptableObject
{
    public string speciesName;
    public string sceneName;
    public List<GameObject> commonEnemies;
    public GameObject bossPrefab;
    public Sprite shipPartIcon;
    public DialogueData introDialogue;
    public DialogueData normalRoundVictoryDialogue;
    public List<DialogueData> bossVictoryDialogues;
}
