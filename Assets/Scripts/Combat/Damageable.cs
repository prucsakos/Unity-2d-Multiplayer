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

    public int HP;
    public int MaxHP = 100;

    private void Start()
    {
        HP = MaxHP;
        uiHealthBar.setDamagable(GetComponent<Damageable>());
    }
    public void TakeDMGServer(int dmg)
    {
        TakeDMG(dmg);
        TakeDMGClientRpc(dmg);
    }

    [ClientRpc]
    void TakeDMGClientRpc(int dmg)
    {
        //if (IsHost) return;
        TakeDMG(dmg);
        
    }

    public void TakeDMG(int dmg)
    {
        HP -= dmg;
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
