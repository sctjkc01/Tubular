using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



namespace UnityStandardAssets.Network
{
	// Subclass this and redefine the function you want
	// then add it to the lobby prefab

	/*
	 *  In the gameplay code you will see where this was subclassed
	 * to do the specific stuff they need for their game
	 * */
	public abstract class LobbyHook : MonoBehaviour
	{
		public virtual void OnLobbyServerSceneLoadedForPlayer (NetworkManager manager, 
		                                                      GameObject lobbyPlayer, 
		                                                      GameObject gamePlayer)
		{
		}
	}

}
