using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public abstract class UpdateObserver: MonoBehaviour {

	public abstract void OnNotify ();
}

public class ScoreboardBehaviour : UpdateObserver {

	private const int MIN_CELL_COUNT = 8;

	[SerializeField]
	private GameObject _loadingContent;
	[SerializeField]
	private GameObject _facebookConnectContent;
	[SerializeField]
	private GameObject _facebookInviteContent;
	[SerializeField]
	private GameObject _scoreboardContent;
	[SerializeField]
	private GameObject _scoreboardListContent;
	[SerializeField]
	private GameObject _scoreboardCellPrefab;


	void Start () {

		PlayerBehaviour.instance.facebookPlayersObserver = this;
	}

	/// <summary>
	/// Action button to log in to Facebook.
	/// </summary>
	public void LogInAction () {

		_loadingContent.SetActive (true);
		_scoreboardContent.SetActive (false);
		_facebookInviteContent.SetActive (false);
		_facebookConnectContent.SetActive (false);

		PlayerBehaviour.instance.Login ();

	}

	/// <summary>
	/// Sets the friend scoreboard active.
	/// </summary>
	public void SetActive () {

		_loadingContent.SetActive (true);
		_scoreboardContent.SetActive (false);
		_facebookInviteContent.SetActive (false);
		_facebookConnectContent.SetActive (false);

		if (FB.IsLoggedIn) {

			PlayerBehaviour.instance.UpdateScoreAndKD ();

		} else {

			_loadingContent.SetActive (false);
			_scoreboardContent.SetActive (false);
			_facebookInviteContent.SetActive (false);
			_facebookConnectContent.SetActive (true);
		}

	}

	/// <summary>
	/// Raises the notify event. Each friends scoreboard list update
	/// </summary>
	public override void OnNotify () {
		UpdateScoreboard ();
	}

	/// <summary>
	/// Update the scoreboard list
	/// </summary>
	void UpdateScoreboard () {

		_loadingContent.SetActive (false);
		_scoreboardContent.SetActive (true);
		_facebookInviteContent.SetActive (true);
		_facebookConnectContent.SetActive (false);

		int count = PlayerBehaviour.instance.facebookPlayers.Count;
		for (int i = 0; i < count; i++) {
			GameObject obj;
			if (_scoreboardListContent.transform.childCount > 0 
				&& _scoreboardListContent.transform.childCount > i) {

				obj = _scoreboardListContent.transform.GetChild (i).gameObject;
			} else {

				obj = Instantiate (_scoreboardCellPrefab);
				obj.transform.SetParent (_scoreboardListContent.transform);
			}

			Player p = PlayerBehaviour.instance.facebookPlayers [i];
			ScoreboardCellBehaviour cell = obj.GetComponent<ScoreboardCellBehaviour> ();
			cell.SetValues (i + 1, p._picture, p._name, p._kd, p._highScore);

		}


		if (count <= MIN_CELL_COUNT) {

			for (int i = count; i < MIN_CELL_COUNT; i++) {
				GameObject cell =  Instantiate (_scoreboardCellPrefab);
				cell.transform.SetParent (_scoreboardListContent.transform);
			}
		}


	}

}
