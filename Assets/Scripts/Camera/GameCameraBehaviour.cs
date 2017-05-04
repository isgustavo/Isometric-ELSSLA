using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraBehaviour : MonoBehaviour {

	private Vector3 offset;
	private Transform target;

	void Update () {

		if (target == null) {
			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

			foreach(GameObject player in players) {
				if (player.GetComponent<ControllerBehaviour>().isLocalPlayer) {
					target = player.transform;
					offset = transform.position - target.position;
					offset = new Vector3 (0, offset.y, offset.z);
				} 
			}
		} 
	}

	void LateUpdate () {

		if (target != null) {
			transform.position = target.position + offset;
		}
	}
}
