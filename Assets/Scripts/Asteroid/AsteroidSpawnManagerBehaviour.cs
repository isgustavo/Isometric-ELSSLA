using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void SpawnFragmentDelegate (GameObject asteroid);

public class AsteroidSpawnManagerBehaviour : NetworkBehaviour {

	private static int ASTEROID_VELOCITY = 5;
	private static int OBJECT_POOL_SIZE = 30;
	private static float TIME_BETWEEN_ASTEROID = 0.5f;

	[SerializeField]
	private GameObject _asteroidPrefab;
	private GameObject[] _pool;

	[SerializeField]
	private FragmentSpawnManagerBehaviour _fragmentSpawnManager;

	void Start() {

		InvokeRepeating ("RespawnFromPool", 0f, TIME_BETWEEN_ASTEROID);

		_pool = new GameObject[OBJECT_POOL_SIZE];
		for (int i = 0; i < OBJECT_POOL_SIZE; ++i) {
			_pool[i] =  (GameObject) Instantiate(_asteroidPrefab, UtilBehaviour.GetRandomAsteroidPosition (), Random.rotation);
			_pool[i].name = "AsteroidPoolObject" + i;

			//Set delegate to call fragments when it is destoyed
			_pool[i].GetComponent<AsteroidBehaviour>()._delegate += new SpawnFragmentDelegate (SpawnFragment);

			//Random Vector3 velocity
			_pool[i].GetComponent<Rigidbody> ().velocity = new Vector3 (
				Random.Range(-ASTEROID_VELOCITY, ASTEROID_VELOCITY) * 0.5f, 
				0f, 
				Random.Range(-ASTEROID_VELOCITY, ASTEROID_VELOCITY) * 0.5f);
			
			_pool[i].SetActive(true);
			_pool[i].transform.parent = this.gameObject.transform;

			NetworkServer.Spawn (_pool[i]);
		}
	}
		

	/// <summary>
	/// Method to respawn asteroid from pool.
	/// </summary>
	void RespawnFromPool () {

		foreach (GameObject obj in _pool) {
			if (!obj.activeInHierarchy) {
				obj.transform.position = UtilBehaviour.GetRandomAsteroidPosition ();
				obj.transform.rotation = Random.rotation;
				obj.transform.localScale = Vector3.one;
				obj.SetActive (true);

				NetworkServer.Spawn (obj);
				break;
			}
		}
	}

	/// <summary>
	/// Delegate method to spawn asteroid fragments.
	/// </summary>
	/// <param name="asteroid">Asteroid destroyed.</param>
	void SpawnFragment(GameObject asteroid) {
		_fragmentSpawnManager.SpawnFragment(asteroid.transform.position);

	}


}
