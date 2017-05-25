using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Facebook.Unity;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;

public class PlayerBehaviour : MonoBehaviour {

	public static PlayerBehaviour instance = null;

	private Player _localPlayer;
	public Player localPlayer { get { return _localPlayer; }}
	private List<Player> _facebookPlayers;
	public List<Player> facebookPlayers { get { return _facebookPlayers; }}

	//Firebase database object
	private DatabaseReference _reference; 

	//GUI observer
	[SerializeField]
	private SetupObserver _observer;

	private UpdateObserver _facebookPlayersObserver;
	public UpdateObserver facebookPlayersObserver { set { _facebookPlayersObserver = value;} }

	void Awake () {
		
		if (instance == null) {

			instance = this;
			// Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
			// http://answers.unity3d.com/questions/30930/why-did-my-binaryserialzer-stop-working.html
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
			DontDestroyOnLoad (this);
		} else if (instance != this) {

			Destroy (gameObject);
		}


	}

	void Start () {
		
		FB.Init (SetInit, OnHideUnity);

	}

	/// <summary>
	/// Sets the init to Facebook Api.
	/// If player logged in going to load from Facebook Api the player data and coins from local storage. See PlayerInfoBasicCallback callback.
	/// If not logged in going to create a dumb player and load the coins from local storage.
	/// me?fields=id,scores 
	///{
	///	"id": "104790426736576",
	///	"scores": {
	///		"data": [
	///			{
	///				"score": 230,
	///				"user": {
	///					"name": "Will Alafafhefgdfe Moidusen",
	///					"id": "104790426736576"
	///				}
	///			}
	///		]
	///	}
	///}
	/// </summary>
	void SetInit() {

		if (FB.IsLoggedIn) {

			FB.API ("me?fields=id,name,scores", HttpMethod.GET, PlayerInfoBasicCallback);
		} else {

			_localPlayer = new Player (LoadCoins());
			_observer.OnNotify ();

		}
	}

	/// <summary>
	/// Raises the hide unity event.
	/// </summary>
	/// <param name="isGameShown">If set to <c>true</c> is game shown.</param>
	void OnHideUnity(bool isGameShown) {

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}

	/// <summary>
	/// Callback to player basic infos from Facebook Api and Facebook Score Api.
	/// </summary>
	/// <param name="result">Callback result</param>
	void PlayerInfoBasicCallback(IResult result) {

		if (result.Error != null) {

			_localPlayer = new Player (LoadCoins());
			_observer.OnNotify ();
		} else {

			string id = result.ResultDictionary["id"].ToString ();
			string name = result.ResultDictionary["name"].ToString ();
			int highscore = 0;
			Coins coins = LoadCoins ();

			if(result.ResultDictionary.ContainsKey ("scores")) {

				IDictionary<string, object> data = (IDictionary<string, object>)result.ResultDictionary["scores"];
				List<object> listObj = (List<object>) data ["data"];

				foreach (object obj in listObj) {

					var entry = (Dictionary<string, object>)obj;

					if (entry ["score"] != null) {
						highscore = Int32.Parse (entry ["score"].ToString ());
					}

					break;
				}
			}

			_localPlayer = new Player (id, name, highscore, LoadCoins ());
			_observer.OnNotify ();
		} 
	}


	/// <summary>
	/// Facebook login request. See AuthCallBack callback.
	/// </summary>
	public void Login () {

		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");

		FB.LogInWithReadPermissions (permissions, AuthCallback);

	}

	/// <summary>
	/// Updates the score and KD.
	/// </summary>
	public void UpdateScoreAndKD () {

		FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, ScoreAndKDCallback);
	}


	/// <summary>
	/// Facebook auths callback. If after callback the player be logged in to Facebook will call PlayerInfoCallback callback.
	/// </summary>
	/// <param name="result">Callback result</param>
	void AuthCallback(IResult result) {

		if (result.Error != null) {

			_facebookPlayersObserver.OnAuthError();
		
		} else {
			if (FB.IsLoggedIn) {

				FB.API ("me?fields=id,name,scores", HttpMethod.GET, PlayerInfoCallback);

			} else {
				_facebookPlayersObserver.OnAuthError();
			}
		}
	}

	/// <summary>
	/// Facebook player basic infos callback. Being the first login is added to coins the gift for logged in.
	/// At last is call the ScoreAndKDCallback callback.
	/// </summary>
	/// <param name="result">Result.</param>
	void PlayerInfoCallback (IResult result) {

		if (result.Error != null) {

			_facebookPlayersObserver.OnError();
		} else {
			
			string id = result.ResultDictionary["id"].ToString ();
			string name = result.ResultDictionary["name"].ToString ();
			int highscore = 0;
			Coins coins = LoadCoins ();

			if (result.ResultDictionary.ContainsKey ("scores")) {

				IDictionary<string, object> data = (IDictionary<string, object>) result.ResultDictionary ["scores"];
				List<object> listObj = (List<object>)data ["data"];

				foreach (object obj in listObj) {

					var entry = (Dictionary<string, object>)obj;

					if (entry ["score"] != null) {
						highscore = Int32.Parse (entry ["score"].ToString ());
					}

					break;
				}
			} else {
				
				coins.SetFacebookGift ();
			}
		
			if (_localPlayer._highScore > highscore) {
				highscore = _localPlayer._highScore;
				SaveNewHighScore (highscore);
			} 

			_localPlayer = new Player (id, name, highscore, coins);

			//Notifies the server that the player has changed id
			foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Player")) {
				ControllerBehaviour player = obj.GetComponent<ControllerBehaviour> ();
				if (player.isLocalPlayer) {
					player.ChangedId ();
					break;
				}
			}

			FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, ScoreAndKDCallback);
		} 
	}

	/// <summary>
	/// Facebook score api callback to load the 30 first facebook friends by score. 
	/// After that is made the Firebase load to get KD values by player.
	/// </summary>
	/// <param name="result">callback result.</param>
	void ScoreAndKDCallback (IResult result) {

		_facebookPlayers = new List<Player> ();

		if (result.ResultDictionary.ContainsKey ("data")) {

			IDictionary<string, object> data = result.ResultDictionary;
			List<object> listObj = (List<object>) data ["data"];

			FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(UtilBehaviour.FIREBASE_REALTIME_DATABASE_PATH);
			_reference = FirebaseDatabase.DefaultInstance.RootReference;

			_reference.Child (UtilBehaviour.ROOT).Child (_localPlayer._id).Child (UtilBehaviour.GROUP).GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {

					_facebookPlayersObserver.OnError();
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;

					foreach (object obj in listObj) {

						var entry = (Dictionary<string, object>) obj;
						var user = (Dictionary<string, object>) entry ["user"];

						string id = user ["id"].ToString ();
						string name = user ["name"].ToString ();
						int highscore = Int32.Parse(entry ["score"].ToString ());
						int k = 0, d = 0;

						if (snapshot.Child(id).Value != null) {
							k = Int32.Parse(snapshot.Child(id).Child("K").Value.ToString ()); 
							d = Int32.Parse(snapshot.Child(id).Child("D").Value.ToString ()); 
						} 

						KD kd = null;
						if (_localPlayer._id != id) {
							kd = new KD (k, d);
						} 

						Player friendPlayer = new Player (id, name, highscore, kd);
						_facebookPlayers.Add (friendPlayer);

					}

					_facebookPlayersObserver.OnNotify();
				}
			});
		}
	}


	/// <summary>
	/// Save on Facebook Score Api the new players high score.
	/// </summary>
	/// <param name="value">High score value</param>
	public void SaveNewHighScore (int value) {
		
		if (_localPlayer._highScore > value) {
			return;
		}

		_localPlayer._highScore = value;

		if (FB.IsLoggedIn) {
			Debug.Log ("SaveNewHighScore"+_localPlayer._highScore);
			var scoreData = new Dictionary<string, string> ();
			scoreData ["score"] = value.ToString ();

			FB.API ("/me/scores", HttpMethod.POST, delegate (IGraphResult result) {
				//Debug.Log ("Set score: " + result.RawResult);
			}, scoreData);
		}
	}

	/// <summary>
	/// Save on Firebase database the new KD. Two request are made to save values by player who was destroyed. 
	/// KD values are key-value below a ID player. That ID is the same use on Facebook Api, so the player need to be logged.
	/// ID Player (local player)
	///    - KD
	///        - ID player (player who destroyed local player)
	///             - K : value
	///             - D : value
	/// 
	/// </summary>
	/// <param name="id">Player id</param>
	public void SaveNewKD (string id) {

		if (!FB.IsLoggedIn) 
			return;

		//Load KD values by foe id
		_reference.Child(UtilBehaviour.ROOT).Child(_localPlayer._id).Child(UtilBehaviour.GROUP).Child(id).GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;

				if (snapshot.Value != null) {

					int k = Int32.Parse(snapshot.Child("K").Value.ToString ()); 
					int d = Int32.Parse(snapshot.Child("D").Value.ToString ()); 

					k += 1;

					//Save new KD to player who destroyed the local player
					KD playerKill = new KD (k, d);
					string json = JsonUtility.ToJson(playerKill);
					_reference.Child(UtilBehaviour.ROOT).Child(_localPlayer._id).Child(UtilBehaviour.GROUP).Child(id).SetRawJsonValueAsync(json);

					//Save new KD to local player
					KD playerDead = new KD (d, k);
					string json2 = JsonUtility.ToJson(playerDead);
					_reference.Child(UtilBehaviour.ROOT).Child(id).Child(UtilBehaviour.GROUP).Child(_localPlayer._id).SetRawJsonValueAsync(json2);

				} else {

					//First interaction between two player
					KD initialKill = new KD (1, 0);
					string json = JsonUtility.ToJson(initialKill);
					_reference.Child(UtilBehaviour.ROOT).Child(_localPlayer._id).Child(UtilBehaviour.GROUP).Child(id).SetRawJsonValueAsync(json);
				
					KD initialDead = new KD (0, 1);
					string json2 = JsonUtility.ToJson(initialDead);
					_reference.Child(UtilBehaviour.ROOT).Child(id).Child(UtilBehaviour.GROUP).Child(_localPlayer._id).SetRawJsonValueAsync(json2);

				}
			}	
		});
	}

	/// <summary>
	/// Uses the coin. Everytime spawn cost coin.
	/// </summary>
	public void UseCoin () {

		_localPlayer._coins.UseCoin ();

		SaveCoins (_localPlayer._coins);
	}

	/// <summary>
	/// Load from local storage the coins data.
	/// </summary>
	/// <returns>Coins loaded.</returns>
	Coins LoadCoins() {
		//File.Delete (Application.persistentDataPath + "/Coins.dat");
		Coins coins;
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/Coins.dat")) {
			file = File.Open (Application.persistentDataPath + "/Coins.dat", FileMode.Open);
			coins = (Coins) bf.Deserialize (file);

			file.Close ();
		} else {
			file = File.Create (Application.persistentDataPath + "/Coins.dat");

			coins = new Coins ();
			bf.Serialize (file, coins);
			file.Close (); 
			SaveCoins (coins);
		}

		return coins;
	}

	/// <summary>
	/// Saves the coins.
	/// </summary>
	/// <param name="coins">Coins.</param>
	void SaveCoins (Coins coins) {

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/Coins.dat")) {

			file = File.Open (Application.persistentDataPath + "/Coins.dat", FileMode.Open);

			bf.Serialize (file, coins);
			file.Close (); 
		} 
	}


	/// <summary>
	/// Raises the application quit event and save coins status
	/// </summary>
	void OnApplicationQuit () {

		SaveCoins (localPlayer._coins);
	}

}