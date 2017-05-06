using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotationJoystickBehaviour : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	private const int _FULL_ANGLE = 360;

	public static RotationJoystickBehaviour instance;

	[SerializeField]
	private Image _content;
	[SerializeField]
	private Image _background;
	[SerializeField]
	private Image _joystick;

	private Vector2 _containerSize;
	private bool _isDragging = false;
	public bool isDragging { get { return _isDragging; } }
	private float _angle = 0f;
	public float angle { get { return _angle; }}

	void Awake() {
	
		if (instance == null) instance = this;
		else if (instance != this) Destroy (gameObject);    
	}

	void Start () {

		_containerSize = GetContainerCenterPoint (_content);

	}

	/// <summary>
	/// Raises the pointer down event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerDown(PointerEventData eventData) {

		_isDragging = true;
		StopAllCoroutines ();
		//Start effect of fade out on controller
		StartCoroutine (FadeTo (15f/255f, 1f, _content));
		StartCoroutine (FadeTo (15f/255f, 1f, _background));
		StartCoroutine (FadeTo (15f/255f, 1f, _joystick));
	}

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnDrag(PointerEventData eventData) {

		Vector2 currentPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (_content.rectTransform,
			eventData.position, eventData.pressEventCamera, out currentPosition)) {

			currentPosition.x = (currentPosition.x / _containerSize.x);
			currentPosition.y = (currentPosition.y / _containerSize.y);

			Vector3 inputPoint = new Vector3 (currentPosition.x * 2 - 1, 0, currentPosition.y * 2 - 1);
			inputPoint = (inputPoint.magnitude > 1.0f) ? inputPoint.normalized : inputPoint;

			_joystick.rectTransform.anchoredPosition = new Vector3 (inputPoint.x * (_containerSize.x / 2), inputPoint.z * (_containerSize.y / 2));

			Vector2 joystickPoint = new Vector2 (_joystick.rectTransform.anchoredPosition.x, _joystick.rectTransform.anchoredPosition.y);

			if ((joystickPoint.x < 0 && joystickPoint.y > 0) || (joystickPoint.x < 0 && joystickPoint.y < 0)) {
				_angle = _FULL_ANGLE - Vector2.Angle (new Vector2 (joystickPoint.x, joystickPoint.y), new Vector2 (0, 1));
			} else {
				_angle = Vector2.Angle (new Vector2 (joystickPoint.x, joystickPoint.y), new Vector2 (0, 1));
			}
		}
	}

	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public virtual void OnPointerUp(PointerEventData eventData) {

		_isDragging = false;
		StopAllCoroutines ();
		//Start effect of fade in controller
		StartCoroutine (FadeTo (40f/255f, 1f, _content));
		StartCoroutine (FadeTo (93f/255f, 1f, _background));
		StartCoroutine (FadeTo (65f/255f, 1f, _joystick));

		_joystick.rectTransform.anchoredPosition = new Vector3 (0, 0);
	}


	/// <summary>
	/// Gets the center point of the container when the object in canvas was created relative the screen size.
	/// </summary>
	/// <param name="obj">Object.</param>
	/// <returns>The center point size.</returns>
	Vector2 GetContainerCenterPoint (Graphic obj) {

		Vector3[] containerCorners = new Vector3[4];
		obj.rectTransform.GetWorldCorners(containerCorners);

		for(int i = 0; i < 4; i++ ) {
			containerCorners[i] = RectTransformUtility.WorldToScreenPoint(null, containerCorners[i]);
		}

		Vector3 bottomLeft = containerCorners[0];
		Vector3 topRight = containerCorners[2];
		float width = topRight.x - bottomLeft.x;
		float height = topRight.y - bottomLeft.y;

		Rect rect = new Rect(bottomLeft.x, topRight.y, width, height);
		return new Vector2(rect.x + rect.width * 0.5f, rect.y - rect.height * 0.5f);
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
