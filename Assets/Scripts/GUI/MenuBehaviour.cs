using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBehaviour : MonoBehaviour {

	public Button button; 

	void Start () {
		StartCoroutine (FadeTo (1f, 2.5f, button.image));

	}

	IEnumerator FadeTo(float aValue, float aTime, Image i) {

		float alpha = i.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {

			Color newColor = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(alpha,aValue,t));
			i.color = newColor;
			yield return null;
		}
	}

}
