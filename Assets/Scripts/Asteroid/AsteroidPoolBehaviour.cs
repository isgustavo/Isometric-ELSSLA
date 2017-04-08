using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidPoolBehaviour : MonoBehaviour {

	private int poolSize = 20;

	public GameObject prefab;
	private Stack<AsteroidBehaviour> asteroidPool = new Stack<AsteroidBehaviour> ();

	void Start() {

		for (int i = 0; i < poolSize; i++) {

			GameObject asteroid = (GameObject)Instantiate (prefab);
			asteroid.transform.parent = gameObject.transform;
			asteroidPool.Push (asteroid.GetComponent<AsteroidBehaviour> ());
		}
	}

	public void Push (AsteroidBehaviour asteroid) {

		asteroidPool.Push (asteroid);
	}

	public AsteroidBehaviour Pop () {

		AsteroidBehaviour asteroid = asteroidPool.Pop ();
		return asteroid;
	}

}
