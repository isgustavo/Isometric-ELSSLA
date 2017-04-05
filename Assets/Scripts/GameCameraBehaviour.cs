using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraBehaviour : MonoBehaviour {

	private Vector3 offset;
	private Transform target;

	void Start () {

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if (player != null) {

			target = player.transform;
			offset = transform.position - target.position;

		} else {

			Debug.LogError("Cant found player object");
			gameObject.SetActive (false);
		}
	}

	void LateUpdate () {

		transform.position = target.position + offset;
	}
}
