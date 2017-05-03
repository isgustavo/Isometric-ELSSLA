using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

[Serializable]
public class Coins {

	private int _count;
	public int count {
		get { return _count; }
	}
	//private DateTime _lastGiftOpen;


	public Coins () {

		_count = 15;
	//	_lastGiftOpen = DateTime.Now;

	}


	public void UseCoin () {

		_count -= 1;
	}

	//public Coins (int count, DateTime lastGiftOpen) {
	//	_count = count;
	//	_lastGiftOpen = lastGiftOpen;

	//}

/*
	public void Save (int score) {

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/PlayerInfo.dat")) {

			file = File.Open (Application.persistentDataPath + "/PlayerInfo.dat", FileMode.Open);

			if (score > player.highscore) {
				player.highscore = score;
			}

			bf.Serialize (file, player);
			file.Close (); 
		} 

	}

	void LoadInfo () {

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file;
		if (File.Exists (Application.persistentDataPath + "/PlayerInfo.dat")) {
			file = File.Open (Application.persistentDataPath + "/PlayerInfo.dat", FileMode.Open);
			player = (PlayerData) bf.Deserialize (file);

			file.Close ();
		} else {
			file = File.Create (Application.persistentDataPath + "/PlayerInfo.dat");

			player = new PlayerData ();
			bf.Serialize (file, player);
			file.Close (); 
		}

	}*/
}
