using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AsteroidBehaviour : NetworkBehaviour, Destructible {

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private ParticleSystem _explosion;
	[SerializeField]
	private Animation _removeAnimation;

	[SyncVar]
	private bool _destoyed = false;
	public event SpawnFragmentDelegate _delegate;


	void Update () {

		if (!isServer)
			return;
		
		if (UtilBehaviour.IsOutOfWorld (gameObject.transform.position) && !_destoyed) {
			_destoyed = true;
			CmdDestroy (true);
		}
	}


	/// <summary>
	/// Asteroid collider.
	/// </summary>
	/// <param name="collision">Object wich asteroid collided.</param>
	void OnCollisionEnter(Collision collision) { 

		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
			CmdDestroy (false);
		}

	}
		
	/// <summary>
	/// Method server-side to remove asteroid from game.
	/// </summary>
	/// <param name="isOutOfArea">When object is out of game area.</param>
	[Command]
	void CmdDestroy (bool isOutOfArea) {

		StartCoroutine (RemoveAndCallFragmentCoroutine(isOutOfArea));
	}

	/// <summary>
	/// Coroutine to remove asteroid from game and call fragment spawn.
	/// </summary>
	IEnumerator RemoveAndCallFragmentCoroutine (bool isOutOfArea) {

		if (isOutOfArea) {
			RpcRemove ();
		} else {
			RpcExplosion ();
			_delegate (this.gameObject);
		}
		yield return new WaitForSeconds (3f);
		RpcUnSpawn ();

	}

	/// <summary>
	/// Rpcs the remove. 
	/// </summary>
	[ClientRpc]
	void RpcRemove () {

		_removeAnimation.Play ();
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
		_destoyed = false;
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
