using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Interface to make objects destructible.
/// </summary>
public interface Destructible {

	int GetPoints ();
}

public class BulletBehaviour : NetworkBehaviour {

	public const float _LIFETIME = .5f;
	public const int _EXPLOSION_LIFETIME = 2;
	public const int _BULLET_VELOCITY = 15;

	[SyncVar]
	private string _id;
	public string id { get{ return _id; } }

	[SyncVar]
	private string _playerName;
	public string playerName { get { return _playerName; } }

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private ParticleSystem _rocket;
	[SerializeField]
	private ParticleSystem _impactExplosion;
	[SerializeField]
	private ParticleSystem _bulletExplosion;

	/// <summary>
	/// Method to setup a new shoot. 
	/// This method should be called from a server-side method.
	/// </summary>
	/// <param name="id">Player's id of who shot</param>
	/// <param name="name">Player's name of who shot</param>
	/// <param name="position">Start position</param>
	/// <param name="rotation">Start rotation</param>
	public void Fire (string id, string name, Vector3 position, Quaternion rotation) {
		_id = id;
		_playerName = name;
		transform.position = position;
		transform.rotation = rotation;

		transform.gameObject.SetActive (true);

		_mesh.SetActive (true);

		//TODO: need implovement, repeated calls to GetComponent() 
		Rigidbody rb = GetComponentInChildren <Rigidbody> ();
		rb.velocity = transform.forward * _BULLET_VELOCITY;	
		rb.angularVelocity = Vector3.zero; 
		_rocket.Play ();

		StartCoroutine (RemoveCoroutine ());
	}

	/// <summary>
	/// Coroutine method to remove bullet when you lifetime expired. 
	/// This method should be called from a server-side method.
	/// </summary>
	IEnumerator RemoveCoroutine () {
		
		yield return new WaitForSeconds(_LIFETIME);
		RpcRemove ();
		yield return new WaitForSeconds (_EXPLOSION_LIFETIME);
		gameObject.SetActive (false);
		NetworkServer.UnSpawn(this.gameObject);
	}

	/// <summary>
	/// Client-side method to sync with all client the bullet removed.
	/// </summary>
	[ClientRpc]
	public void RpcRemove () {

		_mesh.SetActive (false);

		_rocket.Stop ();
		_bulletExplosion.Play ();

	}
		
	/// <summary>
	/// Bullet collider.
	/// </summary>
	/// <param name="collision">Object wich bullet collided.</param>
	void OnCollisionEnter(Collision collision) { 

		StopAllCoroutines ();
		CmdDestroy ();

		//Just the server has a list with all players id. 
		//This id must be use to identify player and send his points.
		if (!isServer)
			return;

		Destructible obj = collision.gameObject.GetComponent<Destructible> ();
		if (obj != null) {
			ControllerBehaviour player = PlayersManager.instance.GetPlayerById (_id);
			player._score += obj.GetPoints ();
		}
			
	}

	/// <summary>
	/// Server-side method to destoy bullet when hit something.
	/// </summary>
	[Command]
	void CmdDestroy () {
		StartCoroutine (DestroyCoroutine ());

	}

	/// <summary>
	/// Coroutine method to destory bullet and unspawn.
	/// This method should be called from a server-side method.
	/// </summary>
	IEnumerator DestroyCoroutine () {
		RpcDestroy ();
		yield return new WaitForSeconds (_EXPLOSION_LIFETIME);
		gameObject.SetActive (false);
		NetworkServer.UnSpawn(gameObject);
	}


	/// <summary>
	/// Client-side method to sync with all client the bullet destroyed.
	/// </summary>
	[ClientRpc]
	public void RpcDestroy () {
		_mesh.SetActive (false);
		_rocket.Stop ();
		_impactExplosion.Play ();
	}

}
