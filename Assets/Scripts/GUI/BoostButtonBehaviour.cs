using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public static BoostButtonBehaviour instance;

	public Image button;

	private bool isPressed;

	void Awake() {

		if (instance == null) instance = this;
		else if (instance != this) Destroy (gameObject);    
	}

	public virtual void OnPointerDown(PointerEventData eventData) {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (0f/255f, .1f, button));

		isPressed = true;
	}

	public virtual void OnPointerUp(PointerEventData eventData) {

		StartCoroutine (FadeTo (95f/255f, .5f, button));

		isPressed = false;
	}

	IEnumerator FadeTo(float aValue, float aTime, Image i) {

		float alpha = i.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {

			Color newColor = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(alpha,aValue,t));
			i.color = newColor;
			yield return null;
		}
	}

	public bool IsPressed () {

		return isPressed;
	}

}
