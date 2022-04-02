using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

/// <summary>
/// Damageable.cs is a networked component of an object that is able to recieve dmg and die.
/// It shall be spawned.
/// </summary>

[RequireComponent(typeof(NetworkObject))]
public class Damageable : NetworkBehaviour
{
    public int hp = 100;
    private void Start()
    {
        
    }

    public void TakeDMGServer(int dmg)
    {
        TakeDMG(dmg);
        TakeDMGClientRpc(dmg);
    }

    [ClientRpc]
    void TakeDMGClientRpc(int dmg)
    {
        TakeDMG(dmg);
    }

    public void TakeDMG(int dmg)
    {
        hp -= dmg;
        if (IsServer && hp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
