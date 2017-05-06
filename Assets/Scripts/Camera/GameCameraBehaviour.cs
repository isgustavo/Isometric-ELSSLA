using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraBehaviour : MonoBehaviour {

	private const int Y_CAMERA = 10;
	private const int Z_CAMERA_OFFSET = 4;
	private const int START_ANIMATION_DURATION = 5;
	private const float IGNORE_LERP = 0.001f;

	private Transform _target;
	private bool _startAnimation = true;


	/// <summary>
	/// Sets the target.
	/// </summary>
	/// <param name="player">Player.</param>
	public void SetTarget (Transform player) {
		_target = player.transform;

	}
		
	void LateUpdate () {

		if (_target != null) {
			if(_startAnimation && Mathf.Abs(transform.position.x) < Mathf.Abs(_target.position.x) - IGNORE_LERP) {
				transform.position = Vector3.Lerp (transform.position, 
					new Vector3 (_target.position.x, 
						Y_CAMERA, _target.position.z - Z_CAMERA_OFFSET), 
					Time.deltaTime * START_ANIMATION_DURATION);
			} else {
				_startAnimation = false;
				transform.position = new Vector3 (_target.position.x, Y_CAMERA, _target.position.z - Z_CAMERA_OFFSET);
			}
		}
	}


}
