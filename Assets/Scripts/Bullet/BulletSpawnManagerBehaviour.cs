using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BulletSpawnManagerBehaviour : MonoBehaviour {

	private static int OBJECT_POOL_SIZE = 12;

	[SerializeField]
	private GameObject _bulletPrefab;

	private NetworkHash128 _assetId;
	public NetworkHash128 assetId {get { return _assetId; }}

	private GameObject[] _pool;

	public delegate GameObject SpawnDelegate(Vector3 position, NetworkHash128 assetId);
	public delegate void UnSpawnDelegate(GameObject spawned);


	void Start() {

		_assetId = _bulletPrefab.GetComponent<NetworkIdentity> ().assetId;
		_pool = new GameObject[OBJECT_POOL_SIZE];
		for (int i = 0; i < OBJECT_POOL_SIZE; ++i) {
			_pool [i] =  (GameObject) Instantiate(_bulletPrefab, Vector3.zero, Quaternion.identity);
			_pool[i].name = "BulletPoolObject" + i;
			_pool[i].SetActive(false);
			_pool [i].transform.parent = this.gameObject.transform;
		}

		ClientScene.RegisterSpawnHandler(_assetId, SpawnObject, UnSpawnObject);
	}


	/// <summary>
	/// Method to get an object from pool.
	/// </summary>
	/// <returns>Return bullet object available to use.</returns>
	public GameObject GetFromPool() {

		foreach (GameObject obj in _pool) {
			if (!obj.activeInHierarchy) {
				return obj;
			}
		}
		return null;
	}

	GameObject SpawnObject(Vector3 position, NetworkHash128 assetId) {
		return GetFromPool();
	}

	void UnSpawnObject(GameObject spawned) {
		spawned.SetActive (false);
	}

}
