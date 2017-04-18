using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MainScenePosition {

	Main,
	Stats

}

public class MenuBehaviour : MonoBehaviour {

	private bool isMenuActive = false;
	private float containerHight;
	private MainScenePosition position;

	public Transform mainSceneContainer;
	public Transform backgroundStars;

	public GameObject title;
	public Button button; 
	public RectTransform menuContainer;
	public Graphic menu;
	public float width ;
	void Start () {
		StartCoroutine (FadeTo (1f, 2.5f, button.image));
		containerHight = GetContainerHight ();

		position = MainScenePosition.Main;

		//mainScenePositions.Add (MainScenePosition.Main, new Vector3 (0, 0, -10));
		//mainScenePositions.Add (MainScenePosition.Stats, new Vector3 (-0.45, 0, -10));

	}

	void Update () {

		UpdateScene ();

	}
	bool done = false;
	void UpdateScene () {

		switch (position) {
		case MainScenePosition.Stats:

			mainSceneContainer.position = Vector3.Lerp (mainSceneContainer.position, new Vector3 (-0.55f, 0f, 0f), Time.deltaTime * 10);
			backgroundStars.rotation = Quaternion.Euler (new Vector3 (-3.13f, -27.1f, 2.789f));

			if (width == 0) {
				width = SetWidth (menu.rectTransform);
			}

			if (menu.rectTransform.anchoredPosition.x > -width && !done) {
				Debug.Log ("Aki");
				menu.rectTransform.anchoredPosition = Vector2.Lerp (menu.rectTransform.anchoredPosition, new Vector2 (-width, 0f), Time.deltaTime * 15);

			} else {
				done = true;
			}

			break;
		}

	}
		
	public void TapToMenu () {

		//isMenuActive = !isMenuActive;
		position = MainScenePosition.Stats;
	}

	public void TapToStats () {

		position = MainScenePosition.Stats;
		//camera.SetScenePosition (mainScenePositions[MainScenePosition.Stats]);
	}

	private float SetWidth (RectTransform position) {

		Vector3[] containerCorners = new Vector3[4];
		position.GetWorldCorners(containerCorners);

		for(int i = 0; i < 4; i++ ) {
			containerCorners[i] = RectTransformUtility.WorldToScreenPoint(null, containerCorners[i]);
		}

		Vector3 bottomLeft = containerCorners[0];
		Vector3 topRight = containerCorners[2];
		return  topRight.x - bottomLeft.x;
		//loat height = topRight.y - bottomLeft.y;



		//Rect rect = new Rect(bottomLeft.x, topRight.y, width, height);
		//containerSize = new Vector2(rect.x + rect.width * 0.5f, rect.y - rect.height * 0.5f);
	}

	private float GetContainerHight () {

		Vector3[] containerCorners = new Vector3[4];
		menuContainer.GetWorldCorners(containerCorners);

		for(int i = 0; i < 4; i++ ) {
			containerCorners[i] = RectTransformUtility.WorldToScreenPoint(null, containerCorners[i]);
		}

		Vector3 bottomLeft = containerCorners[0];
		Vector3 topRight = containerCorners[2];
		return topRight.y - bottomLeft.y;

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
