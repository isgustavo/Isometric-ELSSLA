using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragmentBehaviour : NetworkBehaviour, Destructible {

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private ParticleSystem _explosion;

	/// <summary>
	/// Fragment collider.
	/// </summary>
	/// <param name="collision">Object wich fragment collided.</param>
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

		StartCoroutine (DestroyCoroutine());
	}

	/// <summary>
	/// Coroutine to remove fragment from game.
	/// </summary>
	IEnumerator DestroyCoroutine () {

		RpcExplosion ();
		yield return new WaitForSeconds (3f);
		RpcUnSpawn ();

	}

	/// <summary>
	/// Method client-side to remove fragment mesh and start explosion animation.
	/// </summary>
	[ClientRpc]
	public void RpcExplosion () {

		_mesh.SetActive(false);
		_explosion.Play ();
	}

	/// <summary>
	/// Method client-side to unspawn fragment.
	/// </summary>
	[ClientRpc]
	public void RpcUnSpawn () {

		_mesh.SetActive(true);
		gameObject.SetActive (false);
		NetworkServer.UnSpawn (gameObject);
	}

	/// <summary>
	/// Destructible interface method.
	/// </summary>
	/// <returns>Returns points to destroy fragment.</returns>
	public int GetPoints() {

		return 5;
	}
		
}
