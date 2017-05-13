using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void RespawnDelegate ();

public abstract class DeadObserver : MonoBehaviour {

	public abstract void OnNotify (int score, string name);
}

public class GameSceneBehaviour : DeadObserver {

	[SerializeField]
	private RotationJoystickBehaviour _joystickButton;
	[SerializeField]
	private BoostButtonBehaviour _boostButton;
	[SerializeField]
	private ScoreBehaviour _scorePanel;
	[SerializeField]
	private DeadSceneBehaviour _deadScene;
	[SerializeField]
	private WarningBehaviour _warningContent;

	public RespawnObserver _observer { get; set;}


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


	/// <summary>
	/// Dead Observer.
	/// </summary>
	/// <param name="score">Score.</param>
	/// <param name="name">Name.</param>
	public override void OnNotify (int score, string name) {

		_joystickButton.gameObject.SetActive (false);
		_boostButton.gameObject.SetActive (false);
		_scorePanel.gameObject.SetActive (false);
		_warningContent.gameObject.SetActive (false);

		_deadScene.SetActive (score, name);
		
	}
		
	/// <summary>
	/// Respawns the action.
	/// </summary>
	public void RespawnAction () {

		Init ();
		_observer.OnNotify ();

	}
		

	public void BactToMainMenu () {

		NetworkManager.singleton.client.Disconnect ();
	}

}
