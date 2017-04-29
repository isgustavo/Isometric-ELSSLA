using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour {

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
	private DeadManagerBehaviour _deadManager;

	public void UpdateGUI (State state) {

		switch (state) {

		case State.Game:

			_joystickButton.SetActive (true);
			_boostButton.SetActive (true);
			_scorePanel.SetActive (true);
			_deadManager.gameObject.SetActive (false);

			break;
		case State.Dead:

			_joystickButton.SetActive (false);
			_boostButton.SetActive (false);
			_scorePanel.SetActive (false);
			_deadManager.Active ();

			break;

		}

	}



}
