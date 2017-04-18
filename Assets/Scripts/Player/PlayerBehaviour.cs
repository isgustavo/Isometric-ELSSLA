using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.UI;
using Facebook.Unity;

public class Player {

	private string id;
	private string name;
	private Sprite picture;
	private int highScore;
	private int kill;
	private int death;

	public Player (string id, string name, int highScore, KD kd) {
		this.id = id;
		this.name = name;
		this.highScore = highScore;
		this.kill = kd.kill;
		this.death = kd.death;

		FB.API ("/"+this.id+"/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);

	}

	void ProfilePicCallBack(IGraphResult result) {

		if (result.Texture != null) {

			this.picture = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		} else {
			Debug.Log (result.Error);
		}
	}

}

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

		return kds [key];
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

		IDictionary<string, object> data = result.ResultDictionary;
		List<object> listObj = (List<object>)data ["data"];

		foreach (object obj in listObj) {

			var entry = (Dictionary<string, object>)obj;
			var user = (Dictionary<string, object>)entry ["user"];

			string id = user ["id"].ToString ();
			string name = user ["name"].ToString ();
			string highScore = entry ["score"].ToString ();

			Player player = new Player (id, name, Int32.Parse(highScore), this.data.Get(id));
		    players.Add (id, player);
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


/*
	void UserNameCallBack (IResult result) {

		if (result.Error == null) {

			fbName = result.ResultDictionary ["first_name"].ToString ();

		} else {
			Debug.Log (result.Error);
		}

	}

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
*/


}
