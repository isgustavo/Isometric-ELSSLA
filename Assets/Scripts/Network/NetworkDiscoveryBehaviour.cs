using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkDiscoveryBehaviour : NetworkDiscovery {

	private string serverAddress;
	public ObserverBehaviour observer;

	void Start () {
		Initialize ();
		StartAsClient ();
	}


	public override void OnReceivedBroadcast (string fromAddress, string data) {

		//Network discovery component has found a server being broadcasted
		this.serverAddress = fromAddress;
		Debug.Log ("FOUNDED");
		observer.OnNotify ();
	}

	public string GetAddress () {

		return this.serverAddress;
	}
}
