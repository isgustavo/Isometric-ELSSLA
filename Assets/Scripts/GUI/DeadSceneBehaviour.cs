using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;


public abstract class Observer: MonoBehaviour  {

	public abstract void OnNotify();
}

public class DeadSceneBehaviour : Observer {


	enum ScoreBoardState {

		NoLogIn,
		Loading,
		Active

	}


	[SerializeField]
	private Text _coinText;
	[SerializeField]
	private ScoreBehaviour _newHighScoreContent;

	[SerializeField]
	private Text _nameText;
	[SerializeField]
	private GameObject _killContent;

	[SerializeField]
	private GameObject _facebookConnectContent;
	[SerializeField]
	private GameObject _facebookInviteContent;
	[SerializeField]
	private GameObject _scoreBoardListContent;
	[SerializeField]
	private GameObject _loadingContent;
	[SerializeField]
	private GameObject _scoreBoardCellPrefab;

	public void SetActive (int scored, bool isNewHighScore, string name) {

		_coinText.text = PlayerBehaviour.instance.player.coins.count.ToString ();
		_newHighScoreContent._score = scored;
		_newHighScoreContent.NewHighScore (isNewHighScore);

		if (!name.Equals ("")) {
			_nameText.text = name;
			_killContent.SetActive (true);
		} else {

			_killContent.SetActive (false);
		}

		gameObject.SetActive (true);

		if (FB.IsLoggedIn) {

			UpdateScoreBoard (ScoreBoardState.Loading);
			PlayerBehaviour.instance.AlreadyLogInAction ();
		} else {

			UpdateScoreBoard (ScoreBoardState.NoLogIn);
		}
	}

	void UpdateScoreBoard (ScoreBoardState state) {

		switch (state) {

		case ScoreBoardState.NoLogIn:
			_facebookConnectContent.SetActive (true);
			_facebookInviteContent.SetActive (false);
			_scoreBoardListContent.SetActive (false);
			_loadingContent.SetActive (false);
			break;

		case ScoreBoardState.Loading:
			_facebookConnectContent.SetActive (false);
			_facebookInviteContent.SetActive (false);
			_scoreBoardListContent.SetActive (false);
			_loadingContent.SetActive (true);
			break;

		case ScoreBoardState.Active:

			_facebookConnectContent.SetActive (false);
			_facebookInviteContent.SetActive (true);
			_scoreBoardListContent.SetActive (true);
			_loadingContent.SetActive (false);

			break;

		}
	}

	public void LogInAction () {

		UpdateScoreBoard (ScoreBoardState.Loading);
		PlayerBehaviour.instance.LogInAction ();
	}
		
	public override void OnNotify() {
		UpdateScoreBoard (ScoreBoardState.Active);
		int count = PlayerBehaviour.instance.players.Count;
		for (int i = 0; i < count; i++) {
			GameObject obj;
			if (_scoreBoardListContent.transform.childCount > 0 
				&& _scoreBoardListContent.transform.childCount > i) {

				obj = _scoreBoardListContent.transform.GetChild (i).gameObject;
			} else {
				
				obj = Instantiate (_scoreBoardCellPrefab);
				obj.transform.SetParent (_scoreBoardListContent.transform);
			}

			Player p = PlayerBehaviour.instance.players [i];
			ScoreBoardCell cell = obj.GetComponent<ScoreBoardCell> ();
			cell.SetValues (i + 1, p.picture, p.name, p.k, p.d, p.highScore);

		}


		if (count <= 8) {

			for (int i = count; i < 8; i++) {
				GameObject cell =  Instantiate (_scoreBoardCellPrefab);
				cell.transform.SetParent (_scoreBoardListContent.transform);
			}
		}

	}


}
