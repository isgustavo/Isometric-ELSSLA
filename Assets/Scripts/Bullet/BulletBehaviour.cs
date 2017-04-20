using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class BulletBehaviour : NetworkBehaviour {

	public float m_Lifetime = .7f;
	public float m_ExplosionLifetime = 2f;
	public float m_BulletVelocity = 7.0f;
	public float m_SpreadDelay = .1f;
	public float m_SpreadVelocity = .2f;

	//public Rigidbody rb;
	//private SphereCollider cc;

	//private bool isSpread = false;
	//private Quaternion rotTarget;

	public GameObject m_Mesh;
	public ParticleSystem rocket_PS;
	public ParticleSystem impactExplosion_PS;
	public ParticleSystem bulletExplosion_PS;

	void Start () {
		

	}
		
	//void FixedUpdate () {
		
		//rb.velocity = transform.forward * m_BulletVelocity;	

		//if (isSpread) {
			//rb.rotation = Quaternion.Lerp(rb.rotation, rotTarget, Time.deltaTime * m_SpreadVelocity);
		//}
	//}



	public void Fire () {
		
		m_Mesh.SetActive (true);

		Rigidbody rb = GetComponentInChildren <Rigidbody> ();
		rb.velocity = transform.forward * m_BulletVelocity;	
		rb.angularVelocity = Vector3.zero; 
		rocket_PS.Play ();

		//StartCoroutine(Spread ());
	}

	public IEnumerator Remove() {
		
		yield return new WaitForSeconds(.7f);
		RpcRemove ();
		yield return new WaitForSeconds (2f);
		this.gameObject.SetActive (false);
		NetworkServer.UnSpawn(this.gameObject);
	}

	[ClientRpc]
	public void RpcRemove () {

		m_Mesh.SetActive (false);
		rocket_PS.Stop ();
		bulletExplosion_PS.Play ();
	}

	//IEnumerator Spread () {

	//	yield return new WaitForSeconds(m_SpreadDelay);
	//	Quaternion spreed = Random.rotation;
	//	rotTarget = new Quaternion(0f, 0f, 0f, 0f);
	//	isSpread = true;
	//}

	void OnCollisionEnter(Collision collision) { 

		StopAllCoroutines ();
		StartCoroutine (Destroy ());

	}

	IEnumerator Destroy () {

		RpcDestroy ();
		yield return new WaitForSeconds (2f);
		this.gameObject.SetActive (false);
		NetworkServer.UnSpawn(this.gameObject);
	}

	[ClientRpc]
	public void RpcDestroy () {

		m_Mesh.SetActive (false);
		rocket_PS.Stop ();
		impactExplosion_PS.Play ();
	}

}
