using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour {

	private Rigidbody rb;

	public ParticleSystem explosion;

	void Start () {

		rb = gameObject.GetComponent<Rigidbody> ();
		rb.position = GetRandomPosition ();

	}

	Vector3 GetRandomPosition () {

		return new Vector3 (Random.Range (-12, 12), 6, Random.Range (-12, 12));

	}

}
