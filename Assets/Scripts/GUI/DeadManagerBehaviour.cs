using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.UI;
using Facebook.Unity;




public class DeadManagerBehaviour : Observer {

	private Transform _ScoreBoardTransform;

	[SerializeField]
	private GameObject _noPlayerCellPrefab;
	private GameObject _noPlayerCell;

	[SerializeField]
	private GameObject _facebookConnectCellPrefab;
	private GameObject _facebookConnectCell;

	[SerializeField]
	private GameObject _scoreBoardlistContainer;
	[SerializeField]
	private GameObject _scoreBoardCellPrefab;


	public GameObject m_ScoreBoardContainer;
	public GameObject m_NoPlayerCellPrefab;
	private GameObject m_NoPlayerCell;
	public GameObject m_InviteCell;
	public GameObject m_FacebookConnectCellPrefab;
	private GameObject m_FacebookConnectCell;
	public GameObject m_ScoreBoardCell;

	private ScoreManagerBehaviour scoreManager;

	void Start () {

		scoreManager = gameObject.GetComponent<ScoreManagerBehaviour> ();
	}

	public void SetActive (bool value, int highScore, int lastScore, string name, KD kd) {
		
		gameObject.SetActive (value);
		scoreManager.m_HighScore = highScore;
		scoreManager.m_Score = lastScore;

		if (gameObject.activeInHierarchy) {

			if (FB.IsLoggedIn) {

				m_NoPlayerCell.SetActive (false);
				m_FacebookConnectCell.SetActive (false);


			} else {
				if (m_NoPlayerCell == null) {
					m_NoPlayerCell = Instantiate (m_NoPlayerCellPrefab);
					m_NoPlayerCell.transform.SetParent (_ScoreBoardTransform);
				}

				if (m_FacebookConnectCell == null) {
					m_FacebookConnectCell = Instantiate (m_FacebookConnectCellPrefab);
					m_FacebookConnectCell.transform.SetParent (_ScoreBoardTransform);
				}
			}
		}

	}

	public void Active () {

		gameObject.SetActive (true);

		if (!FB.IsLoggedIn) {

			if (_noPlayerCell == null) {
				_noPlayerCell = Instantiate (_noPlayerCellPrefab);
				_noPlayerCell.transform.SetParent (_ScoreBoardTransform);
			}

			if (_facebookConnectCell == null) {
				_facebookConnectCell = Instantiate (_facebookConnectCellPrefab);
				_facebookConnectCell.transform.SetParent (_ScoreBoardTransform);
			}
		}
	}

	public override void OnNotify() {

		for (int i = 0; i < PlayerBehaviour.instance.players.Count; i++) {
			GameObject obj;
			if (_scoreBoardlistContainer.transform.childCount > 0 && _scoreBoardlistContainer.transform.childCount > i) {
				obj = _scoreBoardlistContainer.transform.GetChild (i).gameObject;

			} else {
				obj = Instantiate (_scoreBoardCellPrefab);
				obj.transform.SetParent (_scoreBoardlistContainer.transform);
			}

			ScoreBoardCell cell = obj.GetComponent<ScoreBoardCell> ();
			//cell.SetValue (players[i]);
		}
	}



	public void LogIn () {

		//PlayerBehaviour.instance.LogIn ();
		//NewHighScore ();
	}

	public void UpdateScoreBoardList () {

		m_NoPlayerCell.SetActive (false);
		m_FacebookConnectCell.SetActive (false);
		/*
		List<Player> players = PlayerBehaviour.instance.GetPlayers ();

		for (int i = 0; i < players.Count; i++) {

			GameObject cell = Instantiate (m_ScoreBoardCell);
			cell.GetComponent<ScoreboardCellBehaviour> ().SetValues (i + 1, players[i].GetName (), 0, players[i].GetK(), players[i].GetD(), players[i].GetHighScore());

			cell.transform.SetParent (m_ScoreBoardContainer.gameObject.transform);
		}

		if (players.Count <= 8) {

			for (int i = players.Count; i < 8; i++) {
				GameObject cell = Instantiate (m_ScoreBoardCell);
				cell.transform.SetParent (m_ScoreBoardContainer.gameObject.transform);
			}
		}

		GameObject inviteCell = Instantiate (m_InviteCell);
		inviteCell.transform.SetParent (m_ScoreBoardContainer.gameObject.transform);
	*/
	}
		
}
