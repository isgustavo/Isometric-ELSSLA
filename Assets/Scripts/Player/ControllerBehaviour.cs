using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : MonoBehaviour {

	private int speed = 10;
	private float timeTilNextShot = .0f;
	private float timeBetweenShot = .3f;

	public ParticleSystem boostExplosion;
	private bool boosted = false;

	private Rigidbody rb;
	private BulletPoolBehaviour bulletPool;

	public Transform bulletPoint;

	void Start () {

		rb = GetComponent<Rigidbody> ();
		bulletPool = GetComponentInChildren<BulletPoolBehaviour> ();
	}

	void Update () {

		if (RotationJoystickBehaviour.instance.IsDragging ()) {
			gameObject.transform.rotation = Quaternion.AngleAxis (RotationJoystickBehaviour.instance.GetAngle (), Vector3.up);

			if (timeTilNextShot < 0) {
				timeTilNextShot = timeBetweenShot;

				BulletBehaviour bullet = bulletPool.Pop ();
				if (bullet != null) {
					bullet.Shot (bulletPoint.position, bulletPoint.rotation);
				}
				//CmdShoot (bulletSpawn.position, bulletSpawn.rotation, gameObject.transform.name);
			}
		}

		timeTilNextShot -= Time.deltaTime;

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
