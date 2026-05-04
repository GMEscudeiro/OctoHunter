using UnityEngine;
using System;

public class PlayerEvents : MonoBehaviour
{
    public event Action<int> OnScoreGained;

    public void AddScore(int amount)
    {
        OnScoreGained?.Invoke(amount);
    }
}
