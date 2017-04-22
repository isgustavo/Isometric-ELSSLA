using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersManager : NetworkBehaviour {

	public static PlayersManager instance;

	private Dictionary<string, ControllerBehaviour> players = new Dictionary<string, ControllerBehaviour> ();

	void Awake () {
		if (instance == null) {

			instance = this;
		} else if (instance != this) {

			Destroy (gameObject);    
		}
	}
		
	public void AddPlayer (string playerId, ControllerBehaviour player) {
		players.Add (playerId, player);

	}

	public ControllerBehaviour GetPlayer (string playerId) {
		return players [playerId];

	}
}
