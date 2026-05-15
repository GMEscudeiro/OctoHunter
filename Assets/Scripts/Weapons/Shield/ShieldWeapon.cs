using UnityEngine;

public class ShieldWeapon : WeaponBase
{
    [Header("Shield Settings")]
    public int bonusHits = 1;

    private PlayerHealth _playerHealth;
    private bool _shieldApplied;

    public override void Initialize(GameObject player)
    {
        base.Initialize(player);

        Debug.Log($"[Shield] Initialize chamado. Player={player.name}");

        // Busca PlayerHealth no próprio objeto, nos filhos, e nos pais
        _playerHealth = player.GetComponent<PlayerHealth>();
        if (_playerHealth == null) _playerHealth = player.GetComponentInChildren<PlayerHealth>();
        if (_playerHealth == null) _playerHealth = player.GetComponentInParent<PlayerHealth>();

        if (_playerHealth != null)
        {
            _playerHealth.AddShield(bonusHits);
            _shieldApplied = true;
            Debug.Log($"[Shield] Escudo aplicado com sucesso! ShieldHits={_playerHealth.ShieldHits}");
        }
        else
        {
            Debug.LogError("[Shield] PlayerHealth NÃO encontrado no player!");
        }
    }

    // Shield is passive — no attack action
    protected override void PerformAttack() { }

    // Override Update to prevent WeaponBase attack loop
    protected override void Update() { }

    private void OnDestroy()
    {
        if (_shieldApplied && _playerHealth != null)
        {
            _playerHealth.RemoveShield(bonusHits);
            _shieldApplied = false;
        }
    }
}
