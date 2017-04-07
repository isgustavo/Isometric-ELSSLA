using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotationJoystickBehaviour : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

	private const int FULL_ANGLE = 360;

	public static RotationJoystickBehaviour instance;

	public Image container;
	public Image background;
	public Image joystick;

	private Vector2 containerSize;
	private float angle;

	void Awake() {
	
		if (instance == null) instance = this;
		else if (instance != this) Destroy (gameObject);    
	}

	void Start () {

		SetContainerSize ();

	}

	public virtual void OnPointerDown(PointerEventData eventData) {


		StopAllCoroutines ();
		StartCoroutine (FadeTo (15f/255f, 1f, container));
		StartCoroutine (FadeTo (15f/255f, 1f, background));
		StartCoroutine (FadeTo (15f/255f, 1f, joystick));
	}

	public virtual void OnDrag(PointerEventData eventData) {

		Vector2 currentPosition;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (container.rectTransform,
			eventData.position, eventData.pressEventCamera, out currentPosition)) {

			currentPosition.x = (currentPosition.x / containerSize.x);
			currentPosition.y = (currentPosition.y / containerSize.y);

			Vector3 inputPoint = new Vector3 (currentPosition.x * 2 - 1, 0, currentPosition.y * 2 - 1);
			inputPoint = (inputPoint.magnitude > 1.0f) ? inputPoint.normalized : inputPoint;

			joystick.rectTransform.anchoredPosition = new Vector3 (inputPoint.x * (containerSize.x / 2), inputPoint.z * (containerSize.y / 2));

			Vector2 joystickPoint = new Vector2 (joystick.rectTransform.anchoredPosition.x, joystick.rectTransform.anchoredPosition.y);

			if ((joystickPoint.x < 0 && joystickPoint.y > 0) || (joystickPoint.x < 0 && joystickPoint.y < 0)) {
				angle = FULL_ANGLE - Vector2.Angle (new Vector2 (joystickPoint.x, joystickPoint.y), new Vector2 (0, 1));
			} else {
				angle = Vector2.Angle (new Vector2 (joystickPoint.x, joystickPoint.y), new Vector2 (0, 1));
			}
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData) {

		StopAllCoroutines ();
		StartCoroutine (FadeTo (40f/255f, 1f, container));
		StartCoroutine (FadeTo (93f/255f, 1f, background));
		StartCoroutine (FadeTo (65f/255f, 1f, joystick));

		joystick.rectTransform.anchoredPosition = new Vector3 (0, 0);
	}


	IEnumerator FadeTo(float aValue, float aTime, Image i) {
		
		float alpha = i.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
			
			Color newColor = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(alpha,aValue,t));
			i.color = newColor;
			yield return null;
		}
	}
		
	private void SetContainerSize () {

		Vector3[] containerCorners = new Vector3[4];
		container.rectTransform.GetWorldCorners(containerCorners);

		for(int i = 0; i < 4; i++ ) {
			containerCorners[i] = RectTransformUtility.WorldToScreenPoint(null, containerCorners[i]);
		}

		Vector3 bottomLeft = containerCorners[0];
		Vector3 topRight = containerCorners[2];
		float width = topRight.x - bottomLeft.x;
		float height = topRight.y - bottomLeft.y;

		Rect rect = new Rect(bottomLeft.x, topRight.y, width, height);
		containerSize = new Vector2(rect.x + rect.width * 0.5f, rect.y - rect.height * 0.5f);
	}


	public float GetAngle ()  {
		return angle;
	}


}
