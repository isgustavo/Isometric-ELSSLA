using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragmentBehaviour : NetworkBehaviour, Destructible {

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private ParticleSystem _explosion;

	[SyncVar]
	private bool _destoyed = false;

	[SerializeField]
	private Animation _removeAnimation;

	void Start () {

		_destoyed = false;
	}

	void Update () {

		if (!isServer)
			return;
		
		if ((UtilBehaviour.IsOutOfWorld (gameObject.transform.position) || 
			UtilBehaviour.IsOutOfY(gameObject.transform.position.y)) && !_destoyed) {

			_destoyed = true;
			CmdDestroy (true);
		}
	}


	/// <summary>
	/// Fragment collider.
	/// </summary>
	/// <param name="collision">Object wich fragment collided.</param>
	void OnCollisionEnter(Collision collision) { 

		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
			CmdDestroy (false);
		}

	}

	/// <summary>
	/// Method server-side to remove asteroid from game.
	/// </summary>
	[Command]
	void CmdDestroy (bool isOutOfArea) {

		StartCoroutine (DestroyCoroutine(isOutOfArea));
	}

	/// <summary>
	/// Coroutine to remove fragment from game.
	/// </summary>
	IEnumerator DestroyCoroutine (bool isOutOfArea) {

		if (isOutOfArea) {
			RpcRemove ();
		} else {
			RpcExplosion ();
		}
		yield return new WaitForSeconds (3f);
		RpcUnSpawn ();

	}

	[ClientRpc]
	public void RpcRemove () {

		_removeAnimation.Play ();
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
		_destoyed = false;
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
