using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    [Header("Data")]
    public WalletData walletData;

    public int Coins => walletData.coins;

    public static event Action<int> OnCoinsChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics() { OnCoinsChanged = null; }

    // Permite que sistemas sem acesso ao componente (ex: CasinoManager) notifiquem a UI
    public static void NotifyChanged(int coins) => OnCoinsChanged?.Invoke(coins);

    void Start()
    {
        OnCoinsChanged?.Invoke(Coins);
    }

    public void AddCoins(int amount)
    {
        walletData.coins += amount;
        OnCoinsChanged?.Invoke(Coins);
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
        return true;
    }
}
