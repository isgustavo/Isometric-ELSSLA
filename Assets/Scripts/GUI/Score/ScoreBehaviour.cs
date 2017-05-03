using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBehaviour : MonoBehaviour {

	private const int INITIAL_SCORE = -1;

	[SerializeField]
	private Text _scoreText;
	[SerializeField]
	private GameObject _newHighScoreContent;

	private int lastScore = INITIAL_SCORE;

	public int _score { get; set; }

	void Update () {

		if (_score >= lastScore) {

			lastScore += Mathf.CeilToInt ((_score - lastScore) * .1f);
			_scoreText.text = lastScore.ToString ("000000000");
		
		}
	}

	public void SetActive (bool value) {
		gameObject.SetActive (value);

		lastScore = INITIAL_SCORE;
		_score = 0;
		NewHighScore (!value);
	}
		
	public void NewHighScore (bool value) {

		_newHighScoreContent.SetActive (value);
	}

}
