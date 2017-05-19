using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OutOfCombatAreaObserver : MonoBehaviour {

	public abstract void OnNotify (bool isOut);
}

public class WarningBehaviour : OutOfCombatAreaObserver {

	private const float _TIME_OUT_TOLERANCE = 6f;
	private float _timeOut = _TIME_OUT_TOLERANCE;

	[SerializeField]
	private Text _countdownText;
	[SerializeField]
	private Animation _animation;

	public delegate void DeadOutOfCombatArea(Collision collision);
	public DeadOutOfCombatArea _delegate { get; set; }

	/// <summary>
	/// Starts the countdown.
	/// </summary>
	void StartCountdown () {

		_timeOut = _TIME_OUT_TOLERANCE;

		InvokeRepeating ("UpdateTimer", 0f, 1f);
	}

	/// <summary>
	/// Stops the countdown.
	/// </summary>
	void StopCountdown () {

		CancelInvoke ("UpdateTimer");
	}

	/// <summary>
	/// Updates the timer.
	/// </summary>
	void UpdateTimer () {

		if (_timeOut <= 0) {
			_delegate (null);
			CancelInvoke ("UpdateTimer");
		}
		_timeOut -= 1;
		_countdownText.text = _timeOut.ToString ("0");
		_animation.Play ();

	}

	/// <summary>
	/// Out of combat area observer
	/// </summary>
	/// <param name="isOut">If set to <c>true</c> is out.</param>
	public override void OnNotify (bool isOut) {

		if (isOut && !gameObject.activeInHierarchy) {

			gameObject.SetActive (true);
			StartCountdown ();
		} else if (!isOut && gameObject.activeInHierarchy) {
			
			StopCountdown ();
			gameObject.SetActive (false);
		}

	}







}
