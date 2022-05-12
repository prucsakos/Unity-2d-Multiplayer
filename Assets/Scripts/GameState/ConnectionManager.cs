using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ConnectionManager : MonoBehaviour
{
    // public int MaxPlayerCount = 3;
    public GameManager gameManager;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }
    // Server side function
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        Debug.Log($"ApprovalCheck || Client ID: {clientId}");
        bool approve = true;
        bool spawnPlayerPrefab = false; 
        callback(spawnPlayerPrefab, null, approve, Vector3.one, Quaternion.identity);
        gameManager.ClientJoined(clientId);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
