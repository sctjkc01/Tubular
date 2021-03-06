﻿using UnityEngine;
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


    //---do we make a var for each player?
    //---i think for points, we should do it based on time on the board
    //---so like 
    //var P1score = p1score + Time.deltatime * 1 * multiplyer  


    private PlayerController localPlayer;

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
            //SetupLoginButtons();
        }
        else
        {
            SetupGameSceneButtons();
        }
    }
    
    void SetupLoginButtons()
    {
        //GameObject.Find("btnHost").GetComponent<Button>().onClick.AddListener(StartupHost);
        
        //GameObject.Find("btnJoin").GetComponent<Button>().onClick.AddListener(JoinGame);
    }
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
		string lanIP = "[unavailable]";
		//CheckPubIP();
		var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
		foreach(var ip in host.AddressList){
			Debug.Log("IP " + ip.AddressFamily);
			if(ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork){
				lanIP = ip.ToString();
			}
		}

        GameObject.Find("Player List Container").GetComponent<Text>().text = "IP " + lanIP;
    }


    public void OnStartClicked()
    {
        if (this.isServer)
        {
            Debug.Log("START");
            GameObject inst = Instantiate(this.gameManagerPrefab);
            NetworkServer.Spawn(inst);
            inst.GetComponent<GameManager>().live = true;
            NetworkServer.SendToAll(StartGameMsg.msgType, new StartGameMsg(Random.Range(int.MinValue, int.MaxValue)));
        }
    }

    //Called on client when connected to server
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        //HandshakeMsg msg = new HandshakeMsg();
        //msg.username = this.username;
        //conn.Send(HandshakeMsg.msgType, msg);
    }

    //Called on server when client disconnects
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        /*foreach (ClientContainer c in this.clients)
        {
            if(c.conn == conn)
            {
                GameObject.Find("Player List Container").GetComponent<Text>().text.Replace(c.username + "\n", "");
                Debug.Log(c.username + " disconnected.");
                this.clients.Remove(c);
                break;
            }
        }*/

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
        this.client.RegisterHandler(HandshakeMsg.msgType, new NetworkMessageDelegate(delegate(NetworkMessage msg)
        {
            HandshakeMsg hmsg = msg.ReadMessage<HandshakeMsg>();
            PlayerController[] players = GameObject.FindObjectsOfType<PlayerController>();
            foreach (PlayerController p in players)
            {
                if (!p.isLocalPlayer || p.playerID == -2) continue;
                p.CmdSetPlayerID(hmsg.connectionID, this.username);//p.playerID = hmsg.connectionID;
                break;
            }
        }));
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

        /*NetworkServer.SendToAll(ClientListMsg.msgType, updateMsg);
        hmsg.connectionID = msg.conn.connectionId;
        msg.conn.Send(HandshakeMsg.msgType, hmsg);*/

        foreach(PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
			string username = null;
			for(int i = 0; i < this.clients.Count; i++)
			{
				if(this.clients[i].conn.connectionId == pc.playerID || (pc.connectionToClient == msg.conn && this.clients[i].conn == msg.conn))
				username = this.clients[i].username;
			}

			if(pc.connectionToClient == msg.conn)
				pc.RpcSetPlayerID(msg.conn.connectionId, username);
			else
            	pc.RpcSetPlayerID(pc.playerID, username);
        }
    }

    #endregion

    #region Network Message Receivers (received on client)

    void OnUpdateClientListReceived(NetworkMessage msg)
    {
        ClientListMsg cmsg = msg.ReadMessage<ClientListMsg>();
        this.usernames = cmsg.usernames;
        //GameObject.Find("Player List Container").GetComponent<Text>().text = string.Join("\n", this.usernames);
    }

    void OnGameStartReceived(NetworkMessage msg)
    {
        StartGameMsg sgm = msg.ReadMessage<StartGameMsg>();
        PathBuilder.SetSeed(sgm.pathSeed);
        GameObject.Find("Shade").SetActive(false);
        GameObject.Find("Lobby").SetActive(false);
    }

    #endregion
    public string Username { get { return username; } }  
}
