using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class ConnectionManager : MonoBehaviour
{
    public GameObject MenuCanvas;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();
        MenuCanvas.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        callback(true, null, true, Vector3.zero, Quaternion.identity);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        MenuCanvas.SetActive(false);
    }
}
