using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : MonoBehaviour {

	private int speed = 10;

	public ParticleSystem boostExplosion;
	private bool boosted = false;

	private Rigidbody rb;

	void Start () {

		rb = GetComponent<Rigidbody> ();
	}

	void Update () {

		gameObject.transform.rotation = Quaternion.AngleAxis(RotationJoystickBehaviour.instance.GetAngle (), Vector3.up);
	}
		
	void FixedUpdate () {

		if (BoostButtonBehaviour.instance.IsPressed ()) {

			if (!boosted) {

				boostExplosion.Play ();
				boosted = true;

				rb.AddForce (transform.forward * speed * 10f, ForceMode.Acceleration);
			} else {

				rb.AddForce (transform.forward * speed, ForceMode.Acceleration);
			}
		} else {

			boosted = false;
		}
	}
}
