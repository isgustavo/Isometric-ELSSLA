using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayersManager : NetworkBehaviour {

	public static PlayersManager instance;

	/// <summary>
	/// Dictionary with player id key and ControllerBehaviour class context as value. 
	/// It is need the ControllerBehaivour context to acess the sync var _score
	/// </summary>
	private Dictionary<string, ControllerBehaviour> players = new Dictionary<string, ControllerBehaviour> ();

	void Awake () {
		if (instance == null) {

			instance = this;
		} else if (instance != this) {

			Destroy (gameObject);    
		}
	}
		
	/// <summary>
	/// Method to add a new player. This method must be call on server-side context
	/// </summary>
	public void AddPlayer (string playerId, ControllerBehaviour player) {
		if (players.ContainsKey (playerId)) {
			players [playerId] = player;
		} else {
			players.Add (playerId, player);
		}

	}

	/// <summary>
	/// Method to get a player with especific id. This method must be call on server-side context
	/// </summary>
	public ControllerBehaviour GetPlayerById (string id) {
		return players [id];

	}
}
