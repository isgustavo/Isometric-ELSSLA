using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreCellBehaviour : MonoBehaviour {

	//image
	[SerializeField]
	private Text position;
	[SerializeField]
	private Text name;
	[SerializeField]
	private Text highScore;

	private Item item;
	private HighScoreListBehaviour scrollList;

	public void SetPosition (int playerPosition) {

		position.text = playerPosition.ToString ();
	}

	public void SetName (string playerName) {

		name.text = playerName;
	}

	public void SetHighScore (int playerScore) {

		highScore.text = playerScore.ToString ();
	}

	public void Setup (Item currentItem, HighScoreListBehaviour currentScrollList) {

		item = currentItem;

		name.text = item.name;

		scrollList = currentScrollList;
	}

}