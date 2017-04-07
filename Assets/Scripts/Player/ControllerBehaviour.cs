using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : MonoBehaviour {

	private int speed = 10;
	private Rigidbody rb;

	void Start () {

		rb = GetComponent<Rigidbody> ();
	}

	void Update () {

		gameObject.transform.rotation = Quaternion.AngleAxis(RotationJoystickBehaviour.instance.GetAngle (), Vector3.up);
	}
		
	void FixedUpdate () {

		if (BoostButtonBehaviour.instance.IsPressed ()) {
			rb.AddForce (transform.forward * speed, ForceMode.Acceleration);
		}
	}
}
