using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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