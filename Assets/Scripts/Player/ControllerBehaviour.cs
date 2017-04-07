using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : MonoBehaviour {

	private Rigidbody rb;

	void Start () {

		rb = GetComponent<Rigidbody> ();
	}

	void Update () {

		gameObject.transform.rotation = Quaternion.AngleAxis(RotationJoystick.instance.GetAngle (), Vector3.up);
	}
		
	void FixedUpdate () {


	}
}
