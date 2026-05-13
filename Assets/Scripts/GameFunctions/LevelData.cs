using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "OctoHunter/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Session Progress")]
    public int totalGlobalRounds = 1;
    public int roundInCurrentSpecies = 1;
    public int currentSpeciesIndex = 0;
    public int collectedShipPartsCount = 0;
    public GameObject shipPartPrefab;

    [Header("Species Pool")]
    public List<SpeciesData> allAvailableSpecies;
    public List<SpeciesData> shuffledSpeciesList;

    public void ResetProgress()
    {
        totalGlobalRounds = 1;
        roundInCurrentSpecies = 1;
        currentSpeciesIndex = 0;
        collectedShipPartsCount = 0;
        shuffledSpeciesList = new List<SpeciesData>();
    }
}
