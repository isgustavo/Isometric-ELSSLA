using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public delegate void SpawnFragmentDelegate (GameObject asteroid);

public class AsteroidSpawnManagerBehaviour : NetworkBehaviour {

	private float timeTilNextAsteroid = .0f;
	private float timeBetweenAsteroid = 5f;

	private int m_ObjectPoolSize = 20;
	public GameObject m_Prefab;
	public GameObject[] m_Pool;

	private FragmentSpawnManagerBehaviour fragmentSpawnManager;

	void Awake () {

		fragmentSpawnManager = GetComponent<FragmentSpawnManagerBehaviour> ();
	}

	void Start() {
		
		m_Pool = new GameObject[m_ObjectPoolSize];
		for (int i = 0; i < m_ObjectPoolSize; ++i) {
			m_Pool[i] =  (GameObject) Instantiate(m_Prefab, GetRandomPosition (), Random.rotation);
			m_Pool[i].name = "AsteroidPoolObject" + i;
			m_Pool[i].GetComponent<AsteroidBehaviour>().m_Delegate += new SpawnFragmentDelegate (this.SpawnFragment);
			m_Pool [i].GetComponent<Rigidbody> ().velocity = new Vector3 (Random.Range(-5, 5) * 0.5f, 0f, Random.Range(-5, 5) * 0.5f);
			m_Pool[i].SetActive(true);
			m_Pool[i].transform.parent = this.gameObject.transform;

			NetworkServer.Spawn (m_Pool[i]);
		}
	}
		
	void Update () {


		if (timeTilNextAsteroid < 0) {
			timeTilNextAsteroid = timeBetweenAsteroid;

			ReSpawnAsteroid ();
		}

		timeTilNextAsteroid -= Time.deltaTime;

	}


	public void ReSpawnAsteroid () {

		ReSpawnFromPool ();
	}

	public void ReSpawnFromPool() {

		foreach (GameObject obj in m_Pool) {
			if (!obj.activeInHierarchy) {
				obj.transform.position = GetRandomPosition ();
				obj.transform.rotation = Random.rotation;
				obj.SetActive (true);

				NetworkServer.Spawn (obj);
			}
		}
	}

	public void SpawnFragment(GameObject asteroid) {
		fragmentSpawnManager.GetFromPool (asteroid.transform.position);

	}

	public Vector3 GetRandomPosition () {

		return new Vector3 (Random.Range (-12, 12), 6, Random.Range (-12, 12));

	}
}
