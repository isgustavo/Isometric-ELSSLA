using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary; 
using System.IO;
using UnityEngine.UI;
using Facebook.Unity;

public class DeadManagerBehaviour : MonoBehaviour {

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

	public void SetActive (bool value, int highScore, int lastScore) {
		
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
					m_NoPlayerCell.transform.SetParent (m_ScoreBoardContainer.gameObject.transform);
				}

				if (m_FacebookConnectCell == null) {
					m_FacebookConnectCell = Instantiate (m_FacebookConnectCellPrefab);
					m_FacebookConnectCell.transform.SetParent (m_ScoreBoardContainer.gameObject.transform);
				}
			}
		}

	}


	public void LogIn () {

		PlayerBehaviour.instance.LogIn ();
	}

	public void UpdateScoreBoardList () {

		m_NoPlayerCell.SetActive (false);
		m_FacebookConnectCell.SetActive (false);

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

	}
		
}
