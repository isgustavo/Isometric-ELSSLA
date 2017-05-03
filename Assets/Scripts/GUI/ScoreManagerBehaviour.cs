using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerBehaviour : MonoBehaviour {

	private const int INITIAL_SCORE = -1;

	[SerializeField]
	private Text _scoreText;
	[SerializeField]
	private GameObject _newHighScoreContent;

	private int lastScore = INITIAL_SCORE;

	public int m_Score { get; set;}
	public int m_HighScore { get; set;}

	void Update () {

		if (m_Score >= lastScore) {

			lastScore += Mathf.CeilToInt ((m_Score - lastScore) * .1f);
			_scoreText.text = lastScore.ToString ("000000000");

			if (lastScore > PlayerBehaviour.instance.player.highScore) {
				_newHighScoreContent.SetActive (true);

			}
		}



	}

}
