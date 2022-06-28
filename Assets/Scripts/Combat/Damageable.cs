using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

/// <summary>
/// Damageable.cs is a networked component of an object that is able to recieve dmg and die.
/// It shall be spawned.
/// </summary>

[RequireComponent(typeof(NetworkObject))]
public class Damageable : NetworkBehaviour
{
    public ulong OwnerId;

    public event EventHandler HpChanged;
    public event EventHandler Died;

    [SerializeField] public UIHealthBar uiHealthBar;

    public NetworkVariable<int> NetHP = new NetworkVariable<int>();
    public NetworkVariable<int> NetMaxHP = new NetworkVariable<int>();
    public int HP;
    public int MaxHP = 100;
    private void Start()
    {
        Died += OnDied;
        uiHealthBar.setDamagable(GetComponent<Damageable>());
    }

    private void OnDied(object sender, EventArgs e)
    {
        // Local Death Logic
    }

    public override void OnNetworkSpawn()
    {
        NetHP.OnValueChanged += OnNetHpChanged;
        if (IsServer || IsHost)
        {
            NetMaxHP.Value = MaxHP;
            NetHP.Value = MaxHP;
            HP = MaxHP;
            HpChanged?.Invoke(this, EventArgs.Empty);
        }
        if (IsClient)
        {
            MaxHP = NetMaxHP.Value;
            HP = NetHP.Value;
            HpChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public void SetHp(int HpToSet)
    {
        if (IsServer || IsHost)
        {
            NetMaxHP.Value = HpToSet;
            NetHP.Value = HpToSet;
            MaxHP = HpToSet;
            HP = HpToSet;
            HpChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public override void OnNetworkDespawn()
    {
        NetHP.OnValueChanged -= OnNetHpChanged;
    }

    private void OnNetHpChanged(int previousValue, int newValue)
    {
        HP = NetHP.Value;
        MaxHP = NetMaxHP.Value;
        HpChanged?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc]
    private void RequestMaxHPServerRpc()
    {
        NetHP.Value = MaxHP;
    }
    public void TakeDMGServer(int dmg)
    {
        TakeDMG(dmg);
    }

    [ClientRpc]
    void TakeDMGClientRpc(int dmg)
    {
        if (IsHost) return;
        TakeDMG(dmg);
        
    }

    public void TakeDMG(int dmg_income)
    {
        int dmg = dmg_income;
        if(TryGetComponent<PlayerController>(out PlayerController pc))
        {
            int shield = Inventory.GetArmorFromNetStruct(pc.NetHelmet.Value, pc.NetArmor.Value);
            dmg -= shield;
            if (dmg < 0) dmg = 0;
        }

        HP -= dmg;
        NetHP.Value = HP;
        NetHP.OnValueChanged?.Invoke(0,0);
        HpChanged?.Invoke(this, EventArgs.Empty);
        if (HP <= 0)
        {
            // Server call
            Died?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public int GetHP()
    {
        return HP;
    }
    public int GetMaxHP()
    {
        return MaxHP;
    }

    public void Regenerate(int am)
    {
        RegenerateServerRpc(am);
    }
	
    [ServerRpc]
    public void RegenerateServerRpc(int am)
    {
        if(NetHP.Value + am > MaxHP)
        {
            NetHP.Value = MaxHP;
        } else
        {
            NetHP.Value += am;
        }
    }
}
