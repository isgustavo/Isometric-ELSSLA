using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraBehaviour : MonoBehaviour {

	private Vector3 offset;
	private Transform target;

	void Update () {

		if (target == null) {
			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			if (player != null) {

				target = player.transform;
				offset = transform.position - target.position;
				offset = new Vector3 (0, offset.y, offset.z);
			} //else {

				//Debug.LogError("Cant found player object");
				//gameObject.SetActive (false);
			//}
		} 

	}

	void LateUpdate () {

		if (target != null) {
			transform.position = target.position + offset;
		}
	}
}
