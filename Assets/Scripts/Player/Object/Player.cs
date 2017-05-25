using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class KD {

	public int _k { get; }
	public int _d { get; }

	public KD (int k, int d) {
		_k = k;
		_d = d;
	}

}

public class Player {

	public const string NO_FB_LOGGED_NAME_PREFIX = "Player";

	//Facebook id from Facebook Api
	public string _id { get; }

	public bool _logged { get; }

	//Facebook name from Facebook Api
	public string _name { get; }

	//Facebook photo from Facebook Api
	public Sprite _picture { get; set; }

	//Facebook score from Facebook Score Api
	public int _highScore { get; set; }

	//KD values from Firebase realtime database Api
	public KD _kd { get; }

	//Coin values from local storage
	public Coins _coins { get; }


	/// <summary>
	/// Initializes a new instance of the <see cref="Player"/> class when player inst logged in to Facebook.
	/// </summary>
	/// <param name="coins">Coins.</param>
	public Player (Coins coins) {

		_id = Player.NO_FB_LOGGED_NAME_PREFIX + UnityEngine.Random.Range (111, 999);
		_logged = false;
		_highScore = 0;
		_coins = coins;
	}
		
	Player (string id, string name) {

		_id = id;
		_name = name;
		_logged = true;

		FB.API ("/"+_id+"/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);
	}

	public Player (string id, string name, int highscore, Coins coins) 
		: this(id, name) {

		_highScore = highscore;
		_coins = coins;
	}

	public Player (string id, string name, int highscore, KD kd) 
		: this(id, name) {

		_highScore = highscore;
		_kd = kd;
	}

	void ProfilePicCallBack(IGraphResult result) {

		if (result.Texture != null) {

			_picture = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		} else {
			Debug.Log (result.Error);
		}
	}

}
	