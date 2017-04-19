using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class BulletBehaviour : NetworkBehaviour {

	public float m_Lifetime = .7f;
	public float m_ExplosionLifetime = .2f;
	public float m_BulletVelocity = 7.0f;
	public float m_SpreadDelay = .1f;
	public float m_SpreadVelocity = .2f;

	private Rigidbody rb;

	private bool isSpread = false;
	private Quaternion rotTarget;

	public GameObject m_Mesh;
	public ParticleSystem rocket_PS;
	public ParticleSystem impactExplosion_PS;
	public ParticleSystem bulletExplosion_PS;

	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
		
		rb.velocity = transform.forward * m_BulletVelocity;	

		if (isSpread) {
			rb.rotation = Quaternion.Lerp(rb.rotation, rotTarget, Time.deltaTime * m_SpreadVelocity);
		}
	}

	public void Fire (Vector3 position, Quaternion rotation) {
		
		transform.position = position;
		transform.rotation = rotation;

		m_Mesh.SetActive (true);
		rocket_PS.Play ();
		StartCoroutine(Spread ());
	}

	[ClientRpc]
	public void RpcRemove () {

		m_Mesh.SetActive (false);
		rocket_PS.Stop ();
		bulletExplosion_PS.Play ();
	}

	IEnumerator Spread () {

		yield return new WaitForSeconds(m_SpreadDelay);
		Quaternion spreed = Random.rotation;
		rotTarget = new Quaternion(0f, spreed.y, 0f, 0f);
		isSpread = true;
	}


}
