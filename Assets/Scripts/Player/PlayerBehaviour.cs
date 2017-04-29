using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Facebook.Unity;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;


public class PlayerBehaviour : MonoBehaviour {

	public static PlayerBehaviour instance = null;

	private Player _player;
	private List<Player> _players;
	public List<Player> players { 
		get { 
			return _players; 
		} 
	}

	private DatabaseReference _reference; 
	private Observer _observer;
	public Observer observer {
		get { return _observer; }
		set { _observer = value; }
	}

	void Awake () {

		if (instance == null) {

			instance = this;

		} else if (instance != this) {

			Destroy (gameObject);
		}
			
		FB.Init (SetInit, OnHideUnity);

	}

	void SetInit() {

		if (FB.IsLoggedIn) {

			//FB.API ("/me?fields=id,first_name", HttpMethod.GET, PlayerInfoCallBack);
		} else {

			Debug.Log ("FB is not logged in");
		}
	}

	void OnHideUnity(bool isGameShown) {

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}


	public void LogInAction () {

		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");

		FB.LogInWithReadPermissions (permissions, AuthCallBack);

	}

	public void NewHighScore (int value) {

		var scoreData = new Dictionary<string, string> ();
		scoreData ["score"] = value.ToString ();

		FB.API ("/me/scores", HttpMethod.POST, delegate (IGraphResult result) {
			Debug.Log("Set score: "+ result.RawResult);
		}, scoreData);
	}

	void AuthCallBack(IResult result) {

		if (result.Error != null) {

			Debug.Log (result.Error);
		} else {
			if (FB.IsLoggedIn) {

				FB.API ("/me?fields=id,first_name", HttpMethod.GET, PlayerInfoCallBack);

			} else {
				Debug.Log ("FB is not logged in");
			}
		}
	}

	void PlayerInfoCallBack (IResult result) {

		if (result.Error != null) {

			Debug.Log (result.Error);
		} else {
			Debug.Log ("PlayerInfoCallBack");
			string id = result.ResultDictionary["id"].ToString ();
			string name = result.ResultDictionary ["first_name"].ToString ();
			_player = new Player (id, name);

			FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, ScoreCallBack);
		} 
	}


	void ScoreCallBack (IResult result) {
		Debug.Log ("ScoreCallBack");
		_players = new List<Player> ();
		IDictionary<string, object> data = result.ResultDictionary;
		List<object> listObj = (List<object>) data ["data"];

		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(UtilBehaviour.FIREBASE_REALTIME_DATABASE_PATH);
		_reference = FirebaseDatabase.DefaultInstance.RootReference;

		_reference.Child (UtilBehaviour.ROOT).Child (_player.id).Child (UtilBehaviour.GROUP).GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				Debug.Log ("Firebase error:" + task.Exception );
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

					if (_player.id == id) {
						_player.highScore = Int32.Parse(highScore);
						_players.Add (_player);
					} else {

						Player player = new Player (id, name, Int32.Parse(highScore), k , d);
						_players.Add (player);
					}
				}
				Debug.Log ("OnNotify");
				_observer.OnNotify();
			}
		});

	}

	/*
	private void SetFriends (List<object> listObj) {

		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://kdfacebookfriends.firebaseio.com/");
		reference = FirebaseDatabase.DefaultInstance.RootReference;

		reference.Child ("users").Child (player.m_Id).Child ("KD").GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				Debug.Log ("Firebase error:" + task.Exception );
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
						k = Int32.Parse(snapshot.Child(id).Child("k").Value.ToString ()); 
						d = Int32.Parse(snapshot.Child(id).Child("d").Value.ToString ()); 
					} 

					if (id == player.m_Id) {
						player.m_Highscore = Int32.Parse(highScore);
						players.Add (player);
					} else {

						Player player = new Player (id, true, name, Int32.Parse(highScore), k , d);
						players.Add (player);
					}
				}

				UpdateList ();

			}
		});


	}

*/

}


/*
[Serializable]
public class KD {

	public int kill;
	public int death;

	public KD (int k, int d) {

		kill = k;
		death = d;
	}
}

[Serializable]
public class KDData {

	Dictionary <string, KD> kds = new Dictionary <string, KD> ();

	public void Add (string k, KD v) {

		kds.Add (k, v);
	}

	public KD Get (string key) {

		if (kds.ContainsKey (key)) {
			return kds [key];
		}
		return new KD (0,0);
	}
}

public class PlayerBehaviour : MonoBehaviour {

	public static PlayerBehaviour instance = null;

	private string fbName;
	private Sprite fbPicture;

	private KDData data = new KDData();
	private Dictionary <string, Player> players = new Dictionary <string, Player> ();

	void Awake () {

		if (instance == null) {

			instance = this;
			// Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
			// http://answers.unity3d.com/questions/30930/why-did-my-binaryserialzer-stop-working.html
			Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		} else if (instance != this) {

			Destroy (gameObject);
		}

		data = LoadData ();
		FB.Init (SetInit, OnHideUnity);

	}
		

	void SetInit() {

		if (FB.IsLoggedIn) {

			LoadFbStats ();
		} else {

			fbName = GetGuestName ();
		}
	}

	void OnHideUnity(bool isGameShown) {

		if (!isGameShown) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}

	}

	public void LogIn () {

		List<string> permissions = new List<string> ();
		permissions.Add ("public_profile");

		FB.LogInWithReadPermissions (permissions, AuthCallBack);
	}

	void AuthCallBack(IResult result) {

		if (result.Error != null) {
			Debug.Log (result.Error);
		} else {
			if (FB.IsLoggedIn) {

				LoadFbStats ();

			} else {
				Debug.Log ("FB is not logged in");
			}
		}
	}

	void LoadFbStats () {

		//FB.API ("/me?fields=first_name", HttpMethod.GET, UserNameCallBack);
		//FB.API ("/me/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);
		FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, FBCallBack);
	}

	private void FBCallBack (IResult result) {
		//result.

		Debug.Log ("Error: "+ result.Error);

		IDictionary<string, object> data = result.ResultDictionary;
		List<object> listObj = (List<object>)data ["data"];

		foreach (object obj in listObj) {

			var entry = (Dictionary<string, object>)obj;
			var user = (Dictionary<string, object>)entry ["user"];

			string id = user ["id"].ToString ();
			string name = user ["name"].ToString ();
			string highScore = entry ["score"].ToString ();

			Debug.Log ("id"+id + " name" + name + "highScore" + highScore);
			Player player = new Player (id, name, Int32.Parse(highScore), this.data.Get(id));
		    players[id] = player;
		}

		GameObject scoreBoardManager = GameObject.FindGameObjectWithTag ("DeadManager");
		if (scoreBoardManager != null) {
			scoreBoardManager.GetComponent<DeadManagerBehaviour> ().UpdateScoreBoardList ();
		}

	}

	KDData LoadData () {

		if (!File.Exists(Application.persistentDataPath + "/SaveData" + ".dat")) {
			
			Debug.Log("File Not Found! Load Failed.");
			return null;
		}

		BinaryFormatter bf = new BinaryFormatter(); 
		FileStream file = File.Open(Application.persistentDataPath + "/SaveData" + ".dat", FileMode.Open); 
		KDData data = (KDData) bf.Deserialize(file); 
		file.Close(); 

		return data;
	}

	void SaveData () {

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/SaveData" + ".dat", FileMode.OpenOrCreate);

		KDData saveData = data;
		bf.Serialize(file, saveData);
		file.Close();
	}

	public string GetShip () {

		return "InitialShip";
	}

	private string GetGuestName () {

		List<string> firstNameSyllables = new List<string>();
		firstNameSyllables.Add("mon");
		firstNameSyllables.Add("fay");
		firstNameSyllables.Add("shi");
		firstNameSyllables.Add("zag");
		firstNameSyllables.Add("blarg");
		firstNameSyllables.Add("rash");
		firstNameSyllables.Add("izen");

		string firstName = "";
		int numberOfSyllablesInFirstName = UnityEngine.Random.Range (2, 4);
		for (int i = 0; i < numberOfSyllablesInFirstName; i++) {
			firstName += firstNameSyllables[UnityEngine.Random.Range(0, firstNameSyllables.Count)];
		}
		string firstNameLetter = "";
		firstNameLetter = firstName.Substring(0,1);
		firstName = firstName.Remove(0,1);
		firstNameLetter = firstNameLetter.ToUpper();
		firstName = firstNameLetter + firstName;
		return "GUEST::"+firstName;
	}

	public string GetName () {

		return fbName;
	}

	public int GetHighScore () {

		return 9999999;
	}


	public List<Player> GetPlayers() {

		return players.Values.ToList<Player> ();
	}

	public KD GetKD (string key) {

		return data.Get (key);
	}

	public void SaveKill (string id) {
		
		KD kd = data.Get (id);

		if (kd != null) {
			kd.kill += 1;
		} else {

			kd = new KD ();
			kd.kill += 1;
			data.Add (id, kd);
		}
	}

	public void SaveDeath (string id) {

		KD kd = data.Get (id);

		if (kd != null) {
			kd.death += 1;
		} else {

			kd = new KD ();
			kd.death += 1;
			data.Add (id, kd);
		}
	}

	void UserNameCallBack (IResult result) {

		if (result.Error == null) {

			fbName = result.ResultDictionary ["first_name"].ToString ();

		} else {
			Debug.Log (result.Error);
		}

	}

/*


	void ProfilePicCallBack(IGraphResult result) {

		if (result.Texture != null) {

			fbPicture = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		} else {
			Debug.Log (result.Error);
		}
	}


	public void SetScore () {

		var scoreData = new Dictionary<string, string> ();
		scoreData ["score"] = "999999990";
		//scoreData.Add ("k", "6");

		FB.API ("/me/scores", HttpMethod.POST, delegate (IGraphResult result) {
			Debug.Log("Set score: "+ result.RawResult);
		}, scoreData);

		FB.API ("/app/scores?fields=score,user.limit(30)", HttpMethod.GET, HighScoreCallBack);
	}



}*/
