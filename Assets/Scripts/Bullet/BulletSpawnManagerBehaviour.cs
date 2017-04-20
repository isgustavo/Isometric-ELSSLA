using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletSpawnManagerBehaviour : MonoBehaviour {

	public int m_ObjectPoolSize = 12;
	public GameObject m_Prefab;
	public GameObject[] m_Pool;

	public NetworkHash128 assetId { get; set; }

	public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
	public delegate void UnSpawnDelegate(GameObject spawned);


	void Start() {
		
		assetId = m_Prefab.GetComponent<NetworkIdentity> ().assetId;
		m_Pool = new GameObject[m_ObjectPoolSize];
		for (int i = 0; i < m_ObjectPoolSize; ++i) {
			m_Pool [i] =  (GameObject) Instantiate(m_Prefab, Vector3.zero, Quaternion.identity);
			m_Pool[i].name = "BulletPoolObject" + i;
			m_Pool[i].SetActive(false);
			m_Pool [i].transform.parent = this.gameObject.transform;
		}

		ClientScene.RegisterSpawnHandler(assetId, SpawnObject, UnSpawnObject);
	}

	public GameObject GetFromPool(Vector3 position, Quaternion rotation) {

		foreach (GameObject obj in m_Pool) {
			if (!obj.activeInHierarchy) {
				Debug.Log("Activating object " + obj.name );
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive (true);
				return obj;
			}
		}

		Debug.LogError ("Could not grab object from pool, nothing available");
		return null;
	}

	public GameObject SpawnObject(Vector3 position, NetworkHash128 assetId) {
		return GetFromPool(position, Quaternion.identity);
	}

	public void UnSpawnObject(GameObject spawned) {
		Debug.Log ("Re-pooling object " + spawned.name);
		spawned.SetActive (false);
	}

}
