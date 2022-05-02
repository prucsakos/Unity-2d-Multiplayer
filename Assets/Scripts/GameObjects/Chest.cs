using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Chest : NetworkBehaviour
{
    public SpriteRenderer sr;
    public Collider2D cd;
    public bool isOpened = false;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        cd = GetComponent<BoxCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("COLLISION HAPPEND");
        if (IsHost && !isOpened)
        {
            Debug.Log("This is host 000");
            if(collision.GetComponent<PlayerController>() != null)
            {
                Debug.Log("Found player");
                OpenChestServerRpc();
            }
        }
    }
    [ServerRpc]
    private void OpenChestServerRpc()
    {
        isOpened = true;
        // spawn items
        ItemWorldSpawner iws = new ItemWorldSpawner(transform.position);
        iws.GenerateDropForChest();
        iws.DropGold();


        OpenChestClientRpc();
    }
    [ClientRpc]
    private void OpenChestClientRpc()
    {
        sr.sprite = ItemAssets.Instance.EmptyChest;
    }

}
