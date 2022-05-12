using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class UIManager : MonoBehaviour
{
    public GameObject Stats;
    public GameObject MapCamera;

    public GameObject MainMenuScreen;
    public GameObject HostScreen;
    public GameObject JoinScreen;

    public GameObject gm;
    private ConnectionManager cm;
    private UNetTransport unt;

    //PARAMS
    public string Port = "7777";
    public string Address = "127.0.0.1";

    private GameObject[] screens;

    private void Start()
    {
        screens = new GameObject[3] { MainMenuScreen, HostScreen, JoinScreen };
        cm = gm.GetComponent<ConnectionManager>();
        unt = NetworkManager.Singleton.GetComponent<UNetTransport>();

        closeScreens();
        MainMenu();
    }
    public void onJoinClicked()
    {
        unt.ConnectAddress = Address;
        unt.ConnectPort = int.Parse(Port);
        cm.Join();
        setupUi();
        closeScreens();
    }
    public void onHostClicked()
    {
        unt.ServerListenPort = int.Parse(Port); 
        cm.Host();
        setupUi();
        closeScreens();
    }
    private void setupUi()
    {
        Stats.SetActive(true);
        //MapCamera.SetActive(false);
    }
    public void onAddressChanged(string value)
    {
        Address = value;
    }
    public void onPortChanged(string value)
    {
        Port = value;
    }
    public void JoinMenu()
    {
        closeScreens();
        JoinScreen.SetActive(true);
    }
    public void HostMenu()
    {
        closeScreens();
        HostScreen.SetActive(true);
    }
    public void MainMenu()
    {
        closeScreens();
        MainMenuScreen.SetActive(true);
    }
    private void closeScreens()
    {
        foreach (GameObject screen in screens)
        {
            screen.SetActive(false);
        }
    }

}
