using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class OutOfCombatAreaObserver : MonoBehaviour {

	public abstract void OnNotify (bool isOut);
}

public class WarningBehaviour : OutOfCombatAreaObserver {

	private const float _TIME_OUT_TOLERANCE = 5f;
	private float _timeOut = _TIME_OUT_TOLERANCE;

	[SerializeField]
	private Text _countDownText;

	public delegate void DeadOutOfCombatArea(Collision collision);
	public DeadOutOfCombatArea _delegate { get; set; }

	void OnEnable () {

		_timeOut = _TIME_OUT_TOLERANCE;
	}


	void Update () {

		if (_timeOut < 0) {
			_delegate (null);
		}
		_timeOut -= Time.deltaTime;
		_countDownText.text = _timeOut.ToString ("0");
	}


	/// <summary>
	/// Out of combat area observer
	/// </summary>
	/// <param name="isOut">If set to <c>true</c> is out.</param>
	public override void OnNotify (bool isOut) {
		
		gameObject.SetActive (isOut);

	}







}
