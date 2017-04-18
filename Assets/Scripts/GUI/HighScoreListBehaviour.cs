using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item {

	public string name;
	public Sprite photo;
	public int highScore;
}

public class HighScoreListBehaviour : MonoBehaviour {

	public List<Item> itemList;
	public Transform contentPanel;

	public GameObject prefab;
	private Stack<GameObject> inactiveInstances = new Stack<GameObject> ();


	void Start () {
		RefreshDisplay ();
	}

	public void RefreshDisplay () {
		
		RemoveItens ();
		AddItens ();
	}

	private void AddItens () {

		for (int i = 0; i < itemList.Count; i++) {

			Item item = itemList[i];
			GameObject newCell = GetObject ();
			newCell.transform.SetParent (contentPanel);

			HighScoreCellBehaviour hsCell = newCell.GetComponent<HighScoreCellBehaviour> ();
			hsCell.Setup (item, this);
		}
	}

	private void RemoveItens () {


		while (contentPanel.childCount > 0) {

			GameObject toRemove = transform.GetChild (0).gameObject;
			ReturnObject (toRemove);
		}
	}

	private void AddItem (Item itemToAdd, HighScoreListBehaviour highScoreList) {

		highScoreList.itemList.Add (itemToAdd);

	}

	private void RemoveItem (Item itemToRemove, HighScoreListBehaviour scrollList) {

		for (int i = scrollList.itemList.Count - 1; i >= 0; i--) {

			if (scrollList.itemList [i] == itemToRemove) {
				scrollList.itemList.RemoveAt (i);
			}
		}

	}


	public GameObject GetObject () {

		GameObject spawnedGameObject;

		if (inactiveInstances.Count != 0) {

			spawnedGameObject = inactiveInstances.Pop ();
		} else {

			spawnedGameObject = (GameObject)GameObject.Instantiate (prefab);
		}

		spawnedGameObject.transform.SetParent (null);
		spawnedGameObject.SetActive (true);

		return spawnedGameObject;

	}

	public void ReturnObject (GameObject toReturn) {
		
		if (inactiveInstances != null) {
			toReturn.transform.SetParent (transform);
			toReturn.SetActive (false);

			inactiveInstances.Push (toReturn);
		} else {

			Destroy (toReturn);
		}
	}

	/*
	void Start () {

		if (LocalPlayerBehaviour.instance.GetHighscoreValuesAvailable ()) {

			int position = 1;
			foreach (HighscorePlayer player in LocalPlayerBehaviour.instance.GetHighscorePlayersList ()) {

				HighscoreListCellBehaviour cell = (Instantiate (highscoreCellPrefab)).GetComponent<HighscoreListCellBehaviour> ();
				cell.gameObject.transform.parent = highscoreContainerPrefab.transform;

				cell.SetPosition (position);
				cell.SetName (player.GetName ());
				cell.SetHighscore (player.GetHighscore ());
				//cell.SetImage ();

				position += 1;
			}


		}
	}

	class ScrollAdapter {


	}
*/
}
