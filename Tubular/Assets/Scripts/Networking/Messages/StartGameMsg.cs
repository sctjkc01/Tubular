using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;


/// <summary>
/// Used as a handshake between client and server.
/// When a client connects to a server, it will immediately send this with its username.
/// </summary>
public class StartGameMsg : MessageBase
{
    public const short msgType = MsgType.Highest + 3;
}
