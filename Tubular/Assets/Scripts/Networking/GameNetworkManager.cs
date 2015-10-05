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
    
    private struct ClientContainer
    {
        public string username;
        public NetworkConnection conn;
    }

    private List<ClientContainer> clients;
    private string[] usernames; //Username list stored on client. Not maintained on server.

    void Start()
    {
        this.SetupLoginButtons();
        clients = new List<ClientContainer>();
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

        this.clients = new List<ClientContainer>();
    }

    public void JoinGame()
    {
        SetIPAddress();
        SetPort();
        SetUsername();
        NetworkManager.singleton.StartClient();
        this.RegisterClientHandlers();

        this.usernames = new string[0];
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
        GameObject.Find("btnHost").GetComponent<Button>().onClick.AddListener(StartupHost);
        
        GameObject.Find("btnJoin").GetComponent<Button>().onClick.AddListener(JoinGame);
    }

    void SetupGameSceneButtons()
    {
        GameObject.Find("btnDisconnect").GetComponent<Button>().onClick.AddListener(Disconnect);
        GameObject.Find("Player List Container").GetComponent<Text>().text = "";
    }


    //Called on client when connected to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        HandshakeMsg msg = new HandshakeMsg();
        msg.username = this.username;
        conn.Send(HandshakeMsg.msgType, msg);
    }

    //Called on server when client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        foreach (ClientContainer c in this.clients)
        {
            if(c.conn == conn)
            {
                GameObject.Find("Player List Container").GetComponent<Text>().text.Replace(c.username + "\n", "");
                Debug.Log(c.username + " disconnected.");
                this.clients.Remove(c);
                break;
            }
        }

        //Updating clients on change
        ClientListMsg updateMsg = new ClientListMsg();
        updateMsg.usernames = new string[this.clients.Count];
        for (int i = 0; i < this.clients.Count; i++)
        {
            updateMsg.usernames[i] = this.clients[i].username;
        }
        NetworkServer.SendToAll(ClientListMsg.msgType, updateMsg);
    }

    //Methods for registering delegates for NetworkMessages
    void RegisterClientHandlers()
    {
        this.client.RegisterHandler(ClientListMsg.msgType, new NetworkMessageDelegate(OnUpdateClientListReceived));
    }

    void RegisterHostHandlers()
    {
        NetworkServer.RegisterHandler(HandshakeMsg.msgType, new NetworkMessageDelegate(OnClientHandshakeReceived));
    }

    #region Network Message Receivers (received on host)
    void OnClientHandshakeReceived(NetworkMessage msg)
    {
        HandshakeMsg hmsg = msg.ReadMessage<HandshakeMsg>();
        ClientContainer c = new ClientContainer();
        c.username = hmsg.username;
        c.conn = msg.conn;
        this.clients.Add(c);

        Debug.Log(hmsg.username + " connected.");
        
        //Updating clients on change and telling newly connected of currently connected
        ClientListMsg updateMsg = new ClientListMsg();
        updateMsg.usernames = new string[this.clients.Count];
        for(int i = 0; i < this.clients.Count; i++)
        {
            updateMsg.usernames[i] = this.clients[i].username;
        }

        NetworkServer.SendToAll(ClientListMsg.msgType, updateMsg);
    }

    #endregion

    #region Network Message Receivers (received on client)

    void OnUpdateClientListReceived(NetworkMessage msg)
    {
        ClientListMsg cmsg = msg.ReadMessage<ClientListMsg>();
        this.usernames = cmsg.usernames;
        GameObject.Find("Player List Container").GetComponent<Text>().text = string.Join("\n", this.usernames);
    }

    #endregion

    public string Username { get { return username; } }
}
