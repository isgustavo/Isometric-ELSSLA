using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : MonoBehaviour {

	[Range(1,9)]
	public int rotateFactor;
	[Range(10,30)]
	public int speedFactor;

	private Rigidbody rb;


	void Start () {

		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {


		Vector3 oldAngles = this.transform.eulerAngles;
		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.LeftArrow)) {
			this.transform.eulerAngles = new Vector3(oldAngles.x, oldAngles.y + (-rotateFactor), oldAngles.z);
		} else if (Input.GetKey(KeyCode.RightArrow)) {
			this.transform.eulerAngles = new Vector3(oldAngles.x, oldAngles.y + (rotateFactor), oldAngles.z);
		}
			
		#else 
		float tiltValue = GetTiltValue();
		this.transform.eulerAngles = new Vector3(oldAngles.x, oldAngles.y, oldAngles.z + (tiltValue * ROTATE_AMOUNT));
		#endif

	}

	void FixedUpdate () {

		#if UNITY_EDITOR
		if (Input.GetKey(KeyCode.UpArrow)) {
			rb.AddForce (transform.forward * speedFactor, ForceMode.Acceleration);
		}
		#endif
	}
}
