using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoostButtonBehaviour : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public static BoostButtonBehaviour instance;

	[SerializeField]
	private Image _button;

	private bool _pressed;
	public bool pressed { get { return _pressed; } }

	void Awake() {

		if (instance == null) instance = this;
		else if (instance != this) Destroy (gameObject);    
	}

	/// <summary>
	/// Raises the pointer down event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerDown(PointerEventData eventData) {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (0f/255f, .1f, _button));

		_pressed = true;
	}

	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerUp(PointerEventData eventData) {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (95f/255f, .5f, _button));

		_pressed = false;
	}

	/// <summary>
	/// Coroutine to change the image's alpha value. 
	/// </summary>
	/// <param name="value">alpha value.</param>
	/// <param name="time">Effect duration</param>
	/// <param name="i">Image target.</param>
	IEnumerator FadeTo(float value, float time, Image i) {

		float alpha = i.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / time) {

			Color newColor = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(alpha, value,t));
			i.color = newColor;
			yield return null;
		}
	}

}
