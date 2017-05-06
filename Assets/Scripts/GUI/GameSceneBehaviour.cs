using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void RespawnDelegate ();

public class GameSceneBehaviour : MonoBehaviour {

	[SerializeField]
	private RotationJoystickBehaviour _joystickButton;
	[SerializeField]
	private BoostButtonBehaviour _boostButton;
	[SerializeField]
	private ScoreBehaviour _scorePanel;
	[SerializeField]
	private DeadSceneBehaviour _deadScene;

	public event RespawnDelegate _delegate;


	void Start () {
		Init ();
	}


	void Init () {
		_joystickButton.gameObject.SetActive (true);
		_joystickButton.OnPointerUp (null);

		_boostButton.gameObject.SetActive (true);
		_boostButton.OnPointerUp (null);

		_scorePanel.gameObject.SetActive (true);
		_scorePanel.Init ();

		_deadScene.gameObject.SetActive (false);
	}


	public void Dead (int scored, bool isNewHighScore, string name) {

		_deadScene.SetActive (scored, isNewHighScore, name);

//		_joystickButton.SetActive (false);
//		_boostButton.SetActive (false);
		_scorePanel.gameObject.SetActive (false);
	}

	public void Respawn () {

		_deadScene.gameObject.SetActive (false);

//		_joystickButton.SetActive (true);
//		_boostButton.SetActive (true);

//		_scorePanel.SetActive (true);

		_delegate ();
	}

	public void SetScore (int score, bool isNewHighScore) {

//		_scorePanel._score = score;
//		_scorePanel.NewHighScore (isNewHighScore);
	}


}
