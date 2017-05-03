using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AsteroidBehaviour : NetworkBehaviour, Destructible {

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private ParticleSystem _explosion;


	public event SpawnFragmentDelegate _delegate;

	/// <summary>
	/// Asteroid collider.
	/// </summary>
	/// <param name="collision">Object wich asteroid collided.</param>
	void OnCollisionEnter(Collision collision) { 

		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
			CmdDestroy ();
		}

	}
		
	/// <summary>
	/// Method server-side to remove asteroid from game.
	/// </summary>
	[Command]
	void CmdDestroy () {

		StartCoroutine (RemoveAndCallFragmentCoroutine());
	}

	/// <summary>
	/// Coroutine to remove asteroid from game and call fragment spawn.
	/// </summary>
	IEnumerator RemoveAndCallFragmentCoroutine () {

		RpcExplosion ();
		_delegate (this.gameObject);
		yield return new WaitForSeconds (3f);
		RpcUnSpawn ();

	}

	/// <summary>
	/// Method client-side to remove asteroid mesh and start explosion animation.
	/// </summary>
	[ClientRpc]
	void RpcExplosion () {

		_mesh.SetActive(false);
		_explosion.Play ();
	}

	/// <summary>
	/// Method client-side to unSpawn asteroid.
	/// </summary>
	[ClientRpc]
	void RpcUnSpawn () {

		_mesh.SetActive(true);
		gameObject.SetActive (false);
		NetworkServer.UnSpawn (gameObject);
	}


	/// <summary>
	/// Destructible interface method.
	/// </summary>
	/// <returns>Returns points to destroy asteroid.</returns>
	public int GetPoints() {

		return 10;
	}
		
}
