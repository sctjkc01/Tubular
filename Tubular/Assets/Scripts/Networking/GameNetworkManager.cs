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
	//might not break it :) but, NetworkLobbyManager is a subclass of Netwrokmanager
    private string username;
    private bool isServer = false;
    [SerializeField]
    private GameObject gameManagerPrefab;

	public GameObject playerInfo;
    
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
        if (this.username.Trim() == "") this.username = "Player";
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
        this.clients = new List<ClientContainer>();
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
	// i think it is beacause we dont have that specific script
    
    void SetupLoginButtons()
    {
        GameObject.Find("btnHost").GetComponent<Button>().onClick.AddListener(StartupHost);
        
        GameObject.Find("btnJoin").GetComponent<Button>().onClick.AddListener(JoinGame);
    }
	//do you know how to add child?
    void SetupGameSceneButtons()
    {
        GameObject.Find("Shade").SetActive(true);
        //GameObject.Find("Lobby").SetActive(true);
		GameObject.Find("btnDisconnect").GetComponent<Button>().onClick.AddListener(Disconnect);
        if(this.isServer) {
          GameObject.Find("btnStart").GetComponent<Button>().onClick.AddListener(OnStartClicked);
          GameObject.Find("btnStart").SetActive(true);
        }else{
          GameObject.Find("btnStart").SetActive(false);
        }
		//577629257929679629289gh4vj894g89fj46h874gh87jk4g86hjk456k941k45
        GameObject.Find("Player List Container").GetComponent<Text>().text = "";
    }


    public void OnStartClicked()
    {
        if (this.isServer)
        {
            Debug.Log("START");
            GameObject inst = Instantiate(this.gameManagerPrefab); // that is similar of what you just said.. This is a ship
            NetworkServer.Spawn(inst);
            inst.GetComponent<GameManager>().live = true;
            NetworkServer.SendToAll(StartGameMsg.msgType, new StartGameMsg(Random.Range(int.MinValue, int.MaxValue)));
        }
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
            {  ///54645641554594141891894189189159156156189156891956
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
        this.client.RegisterHandler(StartGameMsg.msgType, new NetworkMessageDelegate(OnGameStartReceived));
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
		//dfs4gsd5fg41s9d5f1g9sd16fg56s1df56g1s546dg1546sd1fg81sd876fg41s87df1g546sdf1g87sd10fg541sd87fg1s54df1g87s1dfg541sd876f1gsd65fg18
        GameObject.Find("Player List Container").GetComponent<Text>().text = string.Join("\n", this.usernames);
    }

    void OnGameStartReceived(NetworkMessage msg)
    {
        StartGameMsg sgm = msg.ReadMessage<StartGameMsg>();
        PathBuilder.SetSeed(sgm.pathSeed);
        GameObject.Find("Shade").SetActive(false);
        GameObject.Find("Lobby").SetActive(false);
    }

    #endregion
	//594sdh89dg5641d86fgh16df1gh541df68gh1d546fgh1546df1gh41d6fgh16df54gh1546dfgh156d1gh54d6fh
    public string Username { get { return username; } }  
}
