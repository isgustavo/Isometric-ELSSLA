using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public interface Destructible {

	int GetPoints ();
}

public class BulletBehaviour : NetworkBehaviour {

	public float m_Lifetime = .7f;
	public float m_ExplosionLifetime = 2f;
	public float m_BulletVelocity = 12.0f;
	public float m_SpreadDelay = .1f;
	public float m_SpreadVelocity = .2f;

	[SyncVar]
	private string _id;
	public string id { get{ return _id; } }
	[SyncVar]
	private string _playerName;
	public string playerName { get { return _playerName; } }

	public GameObject m_Mesh;
	public ParticleSystem rocket_PS;
	public ParticleSystem impactExplosion_PS;
	public ParticleSystem bulletExplosion_PS;


	public void Fire (string id, string name) {
		this._id = id;
		this._playerName = name;

		m_Mesh.SetActive (true);


		Rigidbody rb = GetComponentInChildren <Rigidbody> ();
		rb.velocity = transform.forward * m_BulletVelocity;	
		rb.angularVelocity = Vector3.zero; 
		rocket_PS.Play ();

	}

	public IEnumerator RemoveCoroutine () {
		
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
		

	void OnCollisionEnter(Collision collision) { 

		if (_id == PlayerBehaviour.instance.player.id) {
			return;
		}

		StopAllCoroutines ();
		CmdDestroy ();

		if (!isServer)
			return;

		GameObject hit = collision.gameObject;
		Destructible obj = hit.GetComponent<Destructible> ();
		if (obj != null) {
			ControllerBehaviour player = PlayersManager.instance.GetPlayer (_id);
			player._score += obj.GetPoints ();
		}
			
	}

	[Command]
	void CmdDestroy () {
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
