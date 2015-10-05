using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[AddComponentMenu("Tubular/Scripts/Networking/Game Network Manager")]
public class GameNetworkManager : NetworkManager
{
    private string username;
    private bool isServer = false;


    void Start()
    {
        this.SetupLoginButtons();
    }

    
    void SetIPAddress()
    {
        string ipAddress = GameObject.Find("inputAddress").GetComponent<InputField>().text;
        if (ipAddress.Trim() == "") ipAddress = "localhost";
        NetworkManager.singleton.networkAddress = ipAddress;
    }

    void SetPort()
    {
        NetworkManager.singleton.networkPort = 10777;
    }

    void SetUsername()
    {
        this.username = GameObject.Find("inputUsername").GetComponent<InputField>().text;
    }
    
    public void StartupHost()
    {
        SetIPAddress();
        SetPort();
        SetUsername();
        this.isServer = true;
        NetworkManager.singleton.StartHost();
        this.RegisterHostHandlers();
        this.RegisterClientHandlers();
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        SetUsername();
        NetworkManager.singleton.StartClient();
        this.RegisterClientHandlers();
    }

    public void Disconnect()
    {
        NetworkManager.singleton.StopHost();
    }
    
    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            SetupLoginButtons();
        }
        else
        {
            SetupGameSceneButtons();
        }
    }

    
    void SetupLoginButtons()
    {
        GameObject.Find("btnHost").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("btnHost").GetComponent<Button>().onClick.AddListener(StartupHost);

        GameObject.Find("btnJoin").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("btnJoin").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupGameSceneButtons()
    {
        GameObject.Find("btnDisconnect").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("btnDisconnect").GetComponent<Button>().onClick.AddListener(NetworkManager.singleton.StopHost);
    }



    //Methods for registering delegates for NetworkMessages
    void RegisterClientHandlers()
    {
    }

    void RegisterHostHandlers()
    {
    }

    public string Username { get { return username; } }
}
