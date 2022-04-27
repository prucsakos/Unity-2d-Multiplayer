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

    public event EventHandler HpChanged;

    [SerializeField] public UIHealthBar uiHealthBar;

    public NetworkVariable<int> NetHP = new NetworkVariable<int>();
    public int HP;
    public int MaxHP = 100;
    private void Start()
    {
        uiHealthBar.setDamagable(GetComponent<Damageable>());
        // ONNETWORKSPAWN
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("ONNETWORKS");
        NetHP.OnValueChanged += OnNetHpChanged;
        if (IsServer)
        {
            NetHP.Value = MaxHP;
            HP = MaxHP;
            HpChanged?.Invoke(this, EventArgs.Empty);
        }
        if (IsClient)
        {
            HP = NetHP.Value;
            HpChanged?.Invoke(this, EventArgs.Empty);
        }
        /*
        if(IsLocalPlayer)
        {
            SyncNetValuesServerRpc();
        }
        */
    }
    public override void OnNetworkDespawn()
    {
        NetHP.OnValueChanged -= OnNetHpChanged;
    }

    private void OnNetHpChanged(int previousValue, int newValue)
    {
        HP = NetHP.Value;
        HpChanged?.Invoke(this, EventArgs.Empty);

    }
    /*
    [ServerRpc(RequireOwnership =false)]
    private void SyncNetValuesServerRpc()
    {
        NetHP.OnValueChanged?.Invoke(HP, HP);
    }
    */
    public void TakeDMGServer(int dmg)
    {
        TakeDMG(dmg);
        //TakeDMGClientRpc(dmg);
    }

    [ClientRpc]
    void TakeDMGClientRpc(int dmg)
    {
        if (IsHost) return;
        TakeDMG(dmg);
        
    }

    public void TakeDMG(int dmg)
    {
        HP -= dmg;
        NetHP.Value = HP;
        NetHP.OnValueChanged?.Invoke(0,0);
        HpChanged?.Invoke(this, EventArgs.Empty);
        if (IsServer && HP <= 0)
        {
            Die();
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
}
