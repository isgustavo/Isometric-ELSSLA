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
	public Player localPlayer { get; }
	private List<Player> _facebookPlayers;

	//Firebase database object
	private DatabaseReference _reference; 

	private Observer _observer;
	public Observer observer {
		get { return _observer; }
		set { _observer = value; }
	}

	void Awake () {

		if (instance == null) {

			instance = this;
			// Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
			// http://answers.unity3d.com/questions/30930/why-did-my-binaryserialzer-stop-working.html
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		} else if (instance != this) {

			Destroy (gameObject);
		}
			
		FB.Init (SetInit, OnHideUnity);

	}

	/// <summary>
	/// Sets the init to Facebook Api.
	/// If player logged in going to load from Facebook Api the player data and coins from local storage. See PlayerInfoBasicCallback callback.
	/// If not logged in going to create a dumb player and load the coins from local storage.
	/// </summary>
	void SetInit() {

		//MenuSceneBehaviour.instance.SetLoading (true);
		if (FB.IsLoggedIn) {

			FB.API ("/me?fields=id,first_name,score", HttpMethod.GET, PlayerInfoBasicCallback);
		} else {

			_localPlayer = new Player (LoadCoins());
			//MenuSceneBehaviour.instance.SetCoinsValue (_player.coins.count);
			//MenuSceneBehaviour.instance.SetLoading (false);
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
			// TODO: DO SAMETHING
		} else {
			
			string id = result.ResultDictionary["id"].ToString ();
			string name = result.ResultDictionary ["first_name"].ToString ();

			IDictionary<string, object> data = (IDictionary<string, object>)result.ResultDictionary["score"];
			List<object> listObj = (List<object>) data ["data"];

			foreach (object obj in listObj) {

				var entry = (Dictionary<string, object>)obj;
				string highScore = entry ["score"].ToString ();

				_localPlayer = new Player (id, name, Int32.Parse(highScore), LoadCoins());
				//MenuSceneBehaviour.instance.SetCoinsValue (_player.coins.count);
				//MenuSceneBehaviour.instance.SetLoading (false);
				break;
			}
		} 
	}


	/// <summary>
	/// Facebook login request. See AuthCallBack callback.
	/// </summary>
	public void LogInAction () {

		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");

		FB.LogInWithReadPermissions (permissions, AuthCallback);

	}

	/// <summary>
	/// Facebook auths callback. If after callback the player be logged in to Facebook will call PlayerInfoCallback callback.
	/// </summary>
	/// <param name="result">Callback result</param>
	void AuthCallback(IResult result) {

		if (result.Error != null) {

			//Debug.Log (result.Error);
			//TODO:
		} else {
			if (FB.IsLoggedIn) {

				FB.API ("/me?fields=id,first_name,score", HttpMethod.GET, PlayerInfoCallback);

			} else {
				//Debug.Log ("FB is not logged in");
				//TODO:
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

			//Debug.Log (result.Error);
			//TODO:
		} else {
			string id = result.ResultDictionary["id"].ToString ();
			string name = result.ResultDictionary ["first_name"].ToString ();
			string highScore = "0";

			IDictionary<string, object> data = (IDictionary<string, object>)result.ResultDictionary["score"];
			List<object> listObj = (List<object>) data ["data"];

			foreach (object obj in listObj) {

				var entry = (Dictionary<string, object>)obj;
				highScore = entry ["score"].ToString ();
				break;
			}

			Coins coins = _localPlayer._coins;
			coins.SetFacebookGift ();

			_localPlayer = new Player (id, name, Int32.Parse (highScore), coins);

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

		IDictionary<string, object> data = result.ResultDictionary;
		List<object> listObj = (List<object>) data ["data"];

		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(UtilBehaviour.FIREBASE_REALTIME_DATABASE_PATH);
		_reference = FirebaseDatabase.DefaultInstance.RootReference;

		_reference.Child (UtilBehaviour.ROOT).Child (_localPlayer._id).Child (UtilBehaviour.GROUP).GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {

				//Debug.Log ("Firebase error:" + task.Exception );
				//TODO:
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;

				foreach (object obj in listObj) {

					var entry = (Dictionary<string, object>) obj;
					var user = (Dictionary<string, object>) entry ["user"];

					string id = user ["id"].ToString ();
					string name = user ["name"].ToString ();
					string highScore = entry ["score"].ToString ();
					int k = 0, d = 0;

					if (snapshot.Child(id).Value != null) {
						k = Int32.Parse(snapshot.Child(id).Child("K").Value.ToString ()); 
						d = Int32.Parse(snapshot.Child(id).Child("D").Value.ToString ()); 
					} 

					KD kd = null;
					if (_localPlayer._id != id) {
						kd = new KD (k, d);
					} 

					Player player = new Player (id, name, Int32.Parse(highScore), kd);
					_facebookPlayers.Add (player);

				}
				//Debug.Log ("OnNotify");
				//_observer.OnNotify();
			}
		});
	}


	/// <summary>
	/// Save on Facebook Score Api the new players high score.
	/// </summary>
	/// <param name="value">High score value</param>
	public void SaveNewHighScore (int value) {

		_localPlayer._highScore = value;

		if (FB.IsLoggedIn) {
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
				//Debug.Log ("Firebase error:" + task.Exception );
				//TODO:
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

}