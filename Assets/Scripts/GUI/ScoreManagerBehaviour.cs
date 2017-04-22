using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManagerBehaviour : MonoBehaviour {

	private const int INITIAL_SCORE = -1;

	public Text scoreText;
	private int lastScore = INITIAL_SCORE;

	public Text highscore;
	private int lastHighscore = INITIAL_SCORE;

	public int m_Score { get; set;}
	public int m_HighScore { get; set;}

	void Update () {

		if (m_Score >= lastScore) {

			lastScore += Mathf.CeilToInt ((m_Score - lastScore) * .1f);
			scoreText.text = lastScore.ToString ("000000000");
		}

		if (m_HighScore  >= lastHighscore) {

			lastHighscore += Mathf.CeilToInt((m_HighScore - lastHighscore) * .1f);
			highscore.text = lastHighscore.ToString("000000000");
		}

	}

}
