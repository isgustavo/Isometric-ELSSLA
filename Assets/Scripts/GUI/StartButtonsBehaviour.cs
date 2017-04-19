using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ObserverBehaviour : MonoBehaviour {
	public abstract void OnNotify ();
}

public class StartButtonsBehaviour : ObserverBehaviour {

	bool t = true;

	public Button tapToPlayButton;
	public Button tapToJoinButton;

	public Text tapToPlayText;
	public Text tapToJoinText;


	void Start () {
		//ShowTapToPlayText ();

		tapToPlayButton.gameObject.SetActive (true);
		tapToJoinButton.gameObject.SetActive (false);
	}
		
	public void ShowTapToJoinText () {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (0f, 0f, tapToPlayText));
		tapToPlayButton.gameObject.SetActive (false);

		tapToJoinButton.gameObject.SetActive(true);
		StartCoroutine (FadeTo (1f, 1.5f, tapToJoinText));
	}

	public void ShowTapToPlayText () {

		StopAllCoroutines ();
		tapToPlayButton.gameObject.SetActive (true);
		StartCoroutine (FadeTo (1f, 1.5f, tapToPlayText));

		StartCoroutine (FadeTo (0f, 0f, tapToJoinText));
		tapToJoinButton.gameObject.SetActive(false);


	}

	IEnumerator FadeTo(float aValue, float aTime, Text t) {
		
		float alpha = t.color.a;
		for (float i = 0.0f; i < 1.0f; i += Time.deltaTime / aTime) {

			Color newColor = new Color(t.color.r, t.color.g, t.color.b, Mathf.Lerp(alpha,aValue,i));
			t.color = newColor;
			yield return null;
		}
	}

	public override void OnNotify () {

		//ShowTapToJoinText ();

		tapToPlayButton.gameObject.SetActive (false);
		tapToJoinButton.gameObject.SetActive (true);
	}

}
