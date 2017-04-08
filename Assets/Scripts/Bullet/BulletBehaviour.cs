using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : MonoBehaviour {

	private float velocity = 7.0f;
	private float spreadDelay = .1f;
	private float spreadVelocity = .2f;

	private bool isActive = false;
	private Rigidbody rb;

	private bool isSpread = false;
	private Quaternion rotTarget;

	public GameObject mesh;
	public ParticleSystem rocket;
	public ParticleSystem impactExplosion;
	public ParticleSystem bulletExplosion;

	void Start () {
		isActive = false;
		rb = GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {

		if (!isActive)
			return;
		
		rb.velocity = transform.forward * velocity;	

		if (isSpread) {

			rb.rotation = Quaternion.Lerp(rb.rotation, rotTarget, Time.deltaTime * spreadVelocity);
		}
	}

	public void Shot (Vector3 position, Quaternion rotation) {
		
		transform.position = position;
		transform.rotation = rotation;
		mesh.SetActive (true);
		isActive = true;
		rocket.Play ();
		StartCoroutine(Spread ());
	}

	public void Remove () {

		mesh.SetActive (false);
		rocket.Stop ();
		bulletExplosion.Play ();
	}

	IEnumerator Spread () {

		yield return new WaitForSeconds(spreadDelay);
		Quaternion spreed = Random.rotation;
		rotTarget = new Quaternion(0f,spreed.y, 0f,0f);
		isSpread = true;
	}


}
