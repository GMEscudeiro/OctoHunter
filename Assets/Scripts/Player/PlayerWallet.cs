using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    [Header("Data")]
    public WalletData walletData;

    public int Coins => walletData.coins;

    public static event Action<int> OnCoinsChanged;

    void Start()
    {
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddCoins(int amount)
    {
        walletData.coins += amount;
        OnCoinsChanged?.Invoke(Coins);
        Debug.Log($"[Wallet] +{amount} moedas. Total: {Coins}");
    }

    public bool SpendCoins(int amount)
    {
        if (walletData.coins < amount)
        {
            Debug.Log($"[Wallet] Saldo insuficiente. Necessário: {amount}, Atual: {Coins}");
            return false;
        }

        walletData.coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        Debug.Log($"[Wallet] -{amount} moedas. Total: {Coins}");
        return true;
    }
}
