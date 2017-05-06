using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class KD {

	private int _k { get; }
	private int _d { get; }

	public KD (int k, int d) {
		_k = k;
		_d = d;
	}

}


public class Player {

	//Facebook id from Facebook Api
	public string _id { get; }

	//Facebook name from Facebook Api
	public string _name { get; }

	//Facebook photo from Facebook Api
	public Sprite _picture { get; set; }

	//Facebook score from Facebook Score Api
	public int _highScore { get; set;}

	//KD values from Firebase realtime database Api
	public KD _kd { get; }

	//Coin values from local storage
	public Coins _coins { get; }


	/// <summary>
	/// Initializes a new instance of the <see cref="Player"/> class when player inst logged in to Facebook.
	/// </summary>
	/// <param name="coins">Coins.</param>
	public Player (Coins coins) {

		_id = "player" + UnityEngine.Random.Range (111, 999);
		_highScore = 0;
		_coins = coins;
	}
		
	Player (string id, string name) {

		_id = id;
		_name = name;

		FB.API ("/"+_id+"/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);
	}

	public Player (string id, string name, int highScore, Coins coins) 
		: this(id, name) {

		_highScore = highScore;
		_coins = coins;
	}

	public Player (string id, string name, int highScore, KD kd) 
		: this(id, name) {

		_highScore = highScore;
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
	