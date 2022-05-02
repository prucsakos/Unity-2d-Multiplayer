using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ConnectionManager : MonoBehaviour
{
    public int MaxPlayerCount = 3;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        int ConnectedCount = NetworkManager.Singleton.ConnectedClientsIds.Count;
        Debug.Log($"Connected count: {ConnectedCount}");
        bool approve = ConnectedCount < MaxPlayerCount ? true : false;
        bool spawnPlayerPrefab = true; 
        callback(spawnPlayerPrefab, null, approve, Vector3.zero, Quaternion.identity);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
