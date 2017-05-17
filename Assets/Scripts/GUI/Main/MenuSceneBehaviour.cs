using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using Facebook.Unity;

public abstract class SetupObserver: MonoBehaviour  {

	public abstract void OnNotify();
}

public class MenuSceneBehaviour : SetupObserver {
	
	[SerializeField]
	private GameObject _loading;

	[SerializeField]
	private GameObject _giftButton;
	[SerializeField]
	private Text _timeLeftText;
	[SerializeField]
	private Animation _giftAnimation;
	private bool _giftAvailable;

	[SerializeField]
	private GameObject _coinsContent;
	[SerializeField]
	private Text _coinsText;
	[SerializeField]
	private Animation _coinsCountAnimation;

	[SerializeField]
	private GameObject _startButtonsContent;


	void Start () {

		if (!FB.IsInitialized) {
			SetLoading ();
		} else {
			OnNotify ();
		}
	}
		
	/// <summary>
	/// Notify event when PlayerBehaviour instance is ready.
	/// </summary>
	public override void OnNotify() {

		RemoveLoading ();
		StartCoroutine("CountDownCoroutine");

		_coinsText.text = PlayerBehaviour.instance.localPlayer._coins.count.ToString ();

	}


	/// <summary>
	/// Coroutine to each minute decrease time for new gift.
	/// </summary>
	IEnumerator CountDownCoroutine() {
		DateTime next = PlayerBehaviour.instance.localPlayer._coins.lastGift.AddDays (1);
		_giftAvailable = false;
		while(true) {
			TimeSpan time = next.Subtract (DateTime.Now);

			if (time.Hours <= 0 && time.Minutes <= 0) {
				_timeLeftText.text = "Get Coin!";
				_giftAnimation.Play ();
				_giftAvailable = true;
				break;
			} else {
				
				_timeLeftText.text = new DateTime(time.Ticks).ToString("HH:mm");
			}

			yield return new WaitForSeconds(60);
		}
	}

	/// <summary>
	/// Button action to get a gift.
	/// </summary>
	public void GetGiftAction () {

		if (_giftAvailable) {
			_giftAnimation.Stop ();
			PlayerBehaviour.instance.localPlayer._coins.SetDailyGift ();
			StartCoroutine("CountDownCoroutine");
			_coinsText.text = PlayerBehaviour.instance.localPlayer._coins.count.ToString ();
			_coinsCountAnimation.Play ();
		}
	}

	/// <summary>
	/// Starts the play action.
	/// </summary>
	public void StartPlayAction () {

		SetLoading ();
		NetworkManagerBehaviour.singleton.StartHost ();
	}

	/// <summary>
	/// Joins the play action.
	/// </summary>
	public void JoinPlayAction () {

		SetLoading ();
		GameObject.FindGameObjectWithTag ("NetworkManager").GetComponent<NetworkManagerBehaviour> ().StartClientPlayer ();
	}

	/// <summary>
	/// Set the loading scene. Just loading animation is active.
	/// </summary>
	void SetLoading () {

		_loading.SetActive (true);

		_giftButton.SetActive (false);
		_startButtonsContent.SetActive (false);
		_coinsContent.SetActive (false);
	}

	/// <summary>
	/// Remove the loading. Every GUI element is active.
	/// </summary>
	void RemoveLoading () {

		_loading.SetActive (false);

		_giftButton.SetActive (true);
		_startButtonsContent.SetActive (true);
		_coinsContent.SetActive (true);
	}
		
}
