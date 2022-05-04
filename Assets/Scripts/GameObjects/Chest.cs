using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Chest : NetworkBehaviour
{
    public SpriteRenderer sr;
    public Collider2D cd;
    public NetworkVariable<bool> isOpened = new NetworkVariable<bool>(false);

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cd = GetComponent<BoxCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isOpened.OnValueChanged += OnIsOpenedChanged;
        if (isOpened.Value)
        {
            sr.sprite = ItemAssets.Instance.EmptyChest;
        }
        else
        {
            sr.sprite = ItemAssets.Instance.ClosedChest;
        }
    }

    private void OnIsOpenedChanged(bool previousValue, bool newValue)
    {
        if (isOpened.Value)
        {
            sr.sprite = ItemAssets.Instance.EmptyChest;
        } else
        {
            sr.sprite = ItemAssets.Instance.ClosedChest;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsHost && !isOpened.Value)
        {
            if(collision.GetComponent<PlayerController>() != null)
            {
                OpenChestServerRpc();
            }
        }
    }
    [ServerRpc]
    private void OpenChestServerRpc()
    {
        isOpened.Value = true;
        // spawn items
        ItemWorldSpawner iws = new ItemWorldSpawner(transform.position);
        iws.GenerateDropForChest();
        iws.DropGold();
    }

    [ClientRpc]
    private void OpenChestClientRpc()
    {
        sr.sprite = ItemAssets.Instance.EmptyChest;
    }

}
