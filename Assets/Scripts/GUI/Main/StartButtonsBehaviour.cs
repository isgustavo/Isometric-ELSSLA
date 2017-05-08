using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ServerObserver : MonoBehaviour {

	public abstract void OnNotify ();
}

public class StartButtonsBehaviour : ServerObserver {

	public Button _playButton;
	public Button _joinButton;


	void Start () {

		_playButton.gameObject.SetActive (true);
		_joinButton.gameObject.SetActive (false);
	}

	public override void OnNotify () {

		_playButton.gameObject.SetActive (false);
		_joinButton.gameObject.SetActive (true);
	}

}
