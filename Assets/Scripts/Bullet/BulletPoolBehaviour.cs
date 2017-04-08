using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPoolBehaviour : MonoBehaviour {

	private float lifetime = .7f;
	private int poolSize = 3;

	public GameObject prefab;
	private Stack<BulletBehaviour> bulletPool = new Stack<BulletBehaviour> ();

	void Start () {

		for (int i = 0; i < poolSize; i++) {

			GameObject bullet = Instantiate (prefab);
			bulletPool.Push (bullet.GetComponent <BulletBehaviour> ());

		}
	}
		
	public BulletBehaviour Pop () {

		BulletBehaviour bullet = bulletPool.Pop ();
		StartCoroutine(Push (bullet));
		return bullet;
	}

	IEnumerator Push (BulletBehaviour bullet) {

		yield return new WaitForSeconds(lifetime);
		bullet.Remove ();

		yield return new WaitForSeconds (bullet.bulletExplosion.time);
		bulletPool.Push (bullet);
	}
}
