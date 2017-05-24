using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ScoreObserver : MonoBehaviour {

	public abstract void OnNotify (int value);
}
	
public class ScoreBehaviour : ScoreObserver {

	private const int INITIAL_SCORE = -1;

	[SerializeField]
	private Text _scoreText;
	[SerializeField]
	private GameObject _newHighScoreContent;

	private int _current_score;
	private int _lastScore;

	/// <summary>
	/// Init the specified score display.
	/// </summary>
	public void Init () {

		_lastScore = INITIAL_SCORE;
		_newHighScoreContent.SetActive (false);
	}

	/// <summary>
	/// Inits the with score. 
	/// </summary>
	/// <param name="score">Score.</param>
	public void InitWithScore (int score) {
		
		_lastScore = INITIAL_SCORE;
		_current_score = score;
	
		if (_current_score >= PlayerBehaviour.instance.localPlayer._highScore) {
			_newHighScoreContent.SetActive (true);
		} else {
			_newHighScoreContent.SetActive (false);
		}
	
	}

	void Update () {

		if (_current_score >= _lastScore) {
			//Score display "animation"
			_lastScore += Mathf.CeilToInt ((_current_score - _lastScore) * .1f);
			_scoreText.text = _lastScore.ToString ("000000000");

		}
	}

	/// <summary>
	/// Raises the notify event. Observer to know when the player has scored
	/// </summary>
	/// <param name="value">New Score</param>
	public override void OnNotify (int value) {

		_current_score = value;

		if (_current_score > PlayerBehaviour.instance.localPlayer._highScore) {
			_newHighScoreContent.SetActive (true);
		}

	}

}
