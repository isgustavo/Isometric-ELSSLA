using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonsBehaviour : MonoBehaviour {

	public bool isAnyPlayerFound;

	public Button tapToPlayText;
	public Button tapToJoinText;


	void Start () {
		ShowTapToJoinText ();
	}
		
		
	public void ShowTapToJoinText () {

		//StopAllCoroutines ();
		//tapToPlayText.CrossFadeAlpha (0f, .1f, true);
		StartCoroutine (FadeTo (1f, 2f, tapToJoinText));
	}

	public void ShowTapToPlayText () {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (255f/255f, 1f, tapToPlayText));
		StartCoroutine (FadeTo (0f/255f, 0f, tapToJoinText));
	}

	IEnumerator FadeTo(float aValue, float aTime, Button t) {

		float alpha = t.image.color.a;
		for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / aTime) {

			Color newColor = new Color(t.image.color.r, t.image.color.g, t.image.color.b, Mathf.Lerp(alpha,aValue,i));
			t.image.color = newColor;
			yield return null;
		}
	}

}
