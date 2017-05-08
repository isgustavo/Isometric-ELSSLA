using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkDiscoveryBehaviour : NetworkDiscovery {

	private string _serverAddress;
	public string serverAddress { get { return _serverAddress; }}
	[SerializeField]
	private ServerObserver observer;

	/// <summary>
	/// Start this instance and start looking for a server.
	/// </summary>
	void Start () {
		Initialize ();
		StartAsClient ();
	}
		
	/// <summary>
	/// Receive broadcast for server found.
	/// </summary>
	/// <param name="fromAddress">Server address.</param>
	/// <param name="data">Data.</param>
	public override void OnReceivedBroadcast (string fromAddress, string data) {

		_serverAddress = fromAddress;
		observer.OnNotify ();
	}

}
