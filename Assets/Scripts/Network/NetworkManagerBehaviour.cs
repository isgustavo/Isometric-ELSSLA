using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkManagerBehaviour : NetworkManager {

	static bool client = false; 

	private const int NETWORK_PORT = 7777;
	public NetworkDiscoveryBehaviour discovery;

	public void OnPlayAction () {
		networkAddress = Network.player.ipAddress;
		networkPort = NETWORK_PORT;
		client = true;
		StartHost ();
	}

	public void OnJoinAction () {
		networkAddress = this.discovery.GetAddress ();

		StartClient ();
	}

	public override void OnStartHost () {
		this.discovery.StopBroadcast ();

		this.discovery.broadcastData = networkPort.ToString ();
		this.discovery.StartAsServer ();
	}

	public override void OnClientSceneChanged(NetworkConnection conn) {
		ClientScene.AddPlayer(conn, 0);
	}

	public override void OnClientConnect(NetworkConnection conn) {
		//base.OnClientConnect(conn);
	}
		
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId) {
		
		foreach (GameObject prefab in spawnPrefabs) {
			
			//if (PlayerBehaviour.instance.GetShip () == prefab.transform.name) {
				GameObject playerShip;
				if (client) {
					client = false;
					playerShip = (GameObject)GameObject.Instantiate (prefab, new Vector3 (-2, 6, -6), Quaternion.identity);

				} else {
					playerShip = (GameObject)GameObject.Instantiate (prefab, new Vector3 (2, 6, -6), Quaternion.identity);
				}

				//playerShip.name = PlayerBehaviour.instance.GetName ();
				NetworkServer.AddPlayerForConnection (conn, playerShip, playerControllerId);
				break;

		//	}
		}
	}


}
