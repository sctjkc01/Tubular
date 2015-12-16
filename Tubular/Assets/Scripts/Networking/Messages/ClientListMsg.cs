using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;


/// <summary>
/// Used to update clients about all connected clients
/// Sent by server whenever a client connects or disconnects
/// </summary>
public class ClientListMsg : MessageBase
{
    public const short msgType = MsgType.Highest + 2;
    public string[] usernames;
	public int yourIDNum;
}
