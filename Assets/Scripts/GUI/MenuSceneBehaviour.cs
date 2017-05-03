using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneBehaviour : MonoBehaviour {

	public static MenuSceneBehaviour instance;

	[SerializeField]
	private GameObject _loading;
	[SerializeField]
	private GameObject _startButtons;
	[SerializeField]
	private Text _coinsText;
	[SerializeField]
	private GameObject _coinsContent;
	[SerializeField]
	private GameObject _settingButton;
	[SerializeField]
	private GameObject _shopButton;
	[SerializeField]
	private GameObject _giftButton;
	[SerializeField]
	private GameObject _videoButton;

	void Awake () {

		if (instance == null) {

			instance = this;

		} else if (instance != this) {

			Destroy (gameObject);
		}
	}


	public void SetLoading (bool value) {

		if (value) {

			_loading.SetActive (true);

			_coinsContent.SetActive (false);
			_startButtons.SetActive (false);

			_settingButton.SetActive (false);
			_shopButton.SetActive (false);
			_giftButton.SetActive (false);
			_videoButton.SetActive (false);

		} else {
			
			_loading.SetActive (false);

			_coinsContent.SetActive (true);
			_startButtons.SetActive (true);

			_settingButton.SetActive (true);
			_shopButton.SetActive (true);
			_giftButton.SetActive (true);
			_videoButton.SetActive (true);

		}
	}
		

	public void SetCoinsValue (int count) {

		_coinsText.text = count.ToString ();
	}
		
}
