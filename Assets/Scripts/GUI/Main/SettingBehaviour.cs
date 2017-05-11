using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class SettingBehaviour : MonoBehaviour {


	[SerializeField]
	private GameObject _popUp;
	[SerializeField]
	private Animator _animator;

	void Start () {

		_popUp.SetActive (false);

	}

	public void OpenSetting () {

		_popUp.SetActive (true);
		_animator.Play ("StartAnimation");

	}

	public void CloseSetting () {

		StartCoroutine (CloseCoroutine ());

	}


	IEnumerator CloseCoroutine () {


		_animator.SetTrigger ("Close");
		yield return new WaitForSeconds (15);
		_popUp.SetActive (false);

	}




	public void FacebookLogout () {

		if (FB.IsLoggedIn) {
			FB.LogOut ();
		}
	}



}
