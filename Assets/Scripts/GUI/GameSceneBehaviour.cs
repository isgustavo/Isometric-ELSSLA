using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneBehaviour : MonoBehaviour {

	public enum State {

		Game,
		Dead
	}

	[SerializeField]
	private GameObject _joystickButton;
	[SerializeField]
	private GameObject _boostButton;
	[SerializeField]
	private GameObject _scorePanel;
	[SerializeField]
	private GameObject _deadScene;
	public GameObject deadScene {
		get { return _deadScene; }
	}

	public void SetState (State state) {

		switch (state) {

		case State.Game:

			//_joystickButton.SetActive (true);
			//_boostButton.SetActive (true);
			//_scorePanel.SetActive (true);
			_deadScene.gameObject.SetActive (false);

			break;
		case State.Dead:

			//_joystickButton.SetActive (false);
			//_boostButton.SetActive (false);
			//_scorePanel.SetActive (false);
			_deadScene.GetComponent<DeadSceneBehaviour>().Active ();

			break;

		}

	}
}
