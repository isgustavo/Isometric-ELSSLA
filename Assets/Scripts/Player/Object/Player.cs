using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class KD {

	private int _k;
	public int k { 
		get { return _k; }
	}
	private int _d;
	public int d {
		get { return _d; }
	}

	public KD (int k, int d) {
		_k = k;
		_d = d;
	}

}


public class Player {

	private string _id;
	public string id { 
		get { return _id; } 
	}
	private string _name;
	public string name { 
		get { return _name; }
	}
	private Sprite _picture;
	public Sprite picture { 
		get { return _picture; }
	}
	private int _highScore;
	public int highScore {
		get { return _highScore; }
		set { _highScore = value; }
	}
	private KD _kd;
	public int k {
		get { return _kd == null ? 0 : _kd.k; }
	}
	public int d {
		get { return _kd == null ? 0 : _kd.d; }
	}

	private Coins _coins;
	public Coins coins {
		get { return _coins; }
	}

	public Player (string id, string name) {

		_id = id;
		_name = name;

		//FB.API ("/"+_id+"/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);
	}

	public Player (string id, string name, int highScore, Coins coins):
		this(id, name) {

		_highScore = highScore;
		_coins = coins;
	}

	public Player (string id, string name, int highScore, int k, int d) 
		: this(id, name, highScore, null){

		_kd = new KD (k, d);
	}

	void ProfilePicCallBack(IGraphResult result) {

		if (result.Texture != null) {

			_picture = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		} else {
			Debug.Log (result.Error);
		}
	}

}



/*
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

	public Player (string id, string name, int highScore) {
		this.id = id;
		this.name = name;
		this.highScore = highScore;

		FB.API ("/"+this.id+"/picture?type=square&height=128&width=128", HttpMethod.GET, ProfilePicCallBack);

	}


	void ProfilePicCallBack(IGraphResult result) {

		if (result.Texture != null) {

			this.picture = Sprite.Create (result.Texture, new Rect (0, 0, 128, 128), new Vector2 ());
		} else {
			Debug.Log (result.Error);
		}
	}

	public string GetName () {
		return name;
	}

	public int GetK () {
		return kill;
	}

	public int GetD () {
		return death;
	}

	public int GetHighScore () {
		return highScore;
	}
}
*/