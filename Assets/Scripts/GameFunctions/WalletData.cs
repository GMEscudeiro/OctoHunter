using UnityEngine;

[CreateAssetMenu(fileName = "WalletData", menuName = "OctoHunter/WalletData")]
public class WalletData : ScriptableObject
{
    public int coins = 0;
    public int currentRound = 1;
}
