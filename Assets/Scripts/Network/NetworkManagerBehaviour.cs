using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerBehaviour : NetworkManager {

	private const int NETWORK_PORT = 7777;
	[SerializeField]
	private NetworkDiscoveryBehaviour _discovery;

	/// <summary>
	/// This event going to start a host player, stop broadcast to find a server and starts other looking for clients.
	/// </summary>
	public override void OnStartHost () {
		networkAddress = Network.player.ipAddress;
		networkPort = NETWORK_PORT;

		_discovery.StopBroadcast ();

		_discovery.broadcastData = networkPort.ToString ();
		_discovery.StartAsServer ();
	}
		
	/// <summary>
	/// Starts the client using the server address by Network Discovery.
	/// </summary>
	public void StartClient() {
		networkAddress = _discovery.serverAddress;
		base.StartClient ();
	}

	public override void OnClientSceneChanged(NetworkConnection conn) {
		ClientScene.AddPlayer(conn, 0);
	}

	public override void OnClientConnect(NetworkConnection conn) {
		//base.OnClientConnect(conn);
	}

	/// <summary>
	/// This method spawn each client joined on server.
	/// </summary>
	/// <param name="conn">Client connection</param>
	/// <param name="playerControllerId">Player controller identifier.</param>
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
		
		foreach (GameObject prefab in spawnPrefabs) {
			
			//TODO: if (PlayerBehaviour.instance.GetShip () == prefab.transform.name) {
				GameObject playerShip = (GameObject)GameObject.Instantiate (prefab, UtilBehaviour.GetRandomPosition (), Quaternion.identity);
		
				NetworkServer.AddPlayerForConnection (conn, playerShip, playerControllerId);
				break;
			//}
		}
	}


}
