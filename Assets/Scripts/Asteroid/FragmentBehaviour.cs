﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragmentBehaviour : NetworkBehaviour, Destructible {

	public GameObject m_Mesh;
	public ParticleSystem explosion_PS;


	void OnCollisionEnter(Collision collision) { 

		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
			CmdDestroy ();
		}

	}

	[Command]
	public void CmdDestroy () {

		StartCoroutine (Destroy());
	}

	IEnumerator Destroy () {

		RpcRemove ();
		yield return new WaitForSeconds (3f);
		RpcUnSpawn ();

	}

	[ClientRpc]
	public void RpcRemove () {

		m_Mesh.SetActive(false);
		explosion_PS.Play ();
	}

	[ClientRpc]
	public void RpcUnSpawn () {

		m_Mesh.SetActive(true);
		this.gameObject.SetActive (false);
		NetworkServer.UnSpawn (this.gameObject);
	}

	public int GetPoints() {

		return 5;
	}
		
}