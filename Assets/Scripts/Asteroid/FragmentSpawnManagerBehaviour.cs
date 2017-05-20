using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragmentSpawnManagerBehaviour : MonoBehaviour {

	private const int FRAGMENT_VELOCITY = 5;

	//random possibility to spawn fragent with 2 or 3 parts
	private const int _RANGE_FROM = 0;
	private const int _RANGE_TO = 10;
	private const int _FRAGMENT_PROPORTION = 6;

	//Initial fragment with 2 part position
	private static readonly Vector3 _INITIAL_POSITION_FRAG_1_2 = new Vector3(-.25f, 0f, 0f);
	private static readonly Vector3 _INITIAL_POSITION_FRAG_2_2 = new Vector3(.25f, 0f, 0f);

	//Initial fragment with 3 part position and rotation
	private static readonly Vector3 _INITIAL_POSITION_FRAG_1_3 = new Vector3(0f, .35f, .093f);
	private static readonly Quaternion _INITIAL_ROTATION_FRAG_1_3 = new Quaternion (37, 0, 0, 0);

	private static readonly Vector3 _INITIAL_POSITION_FRAG_2_3 = new Vector3(0f, 0f, 0f);
	private static readonly Quaternion _INITIAL_ROTATION_FRAG_2_3 = new Quaternion (60, 0, 0, 0);

	private static readonly Vector3 _INITIAL_POSITION_FRAG_3_3 = new Vector3(0f, -.058f, -.309f);
	private static readonly Quaternion _INITIAL_ROTATION_FRAG_3_3 = new Quaternion (62, 0, 0, 0);

	[SerializeField]
	private GameObject _frag12Prefab;
	[SerializeField]
	private GameObject _frag22Prefab;
	[SerializeField]
	private GameObject _frag13Prefab;
	[SerializeField]
	private GameObject _frag23Prefab;
	[SerializeField]
	private GameObject _frag33Prefab;

	private List<GameObject> _frag12Pool = new List<GameObject> ();
	private List<GameObject> _frag22Pool = new List<GameObject> ();

	private List<GameObject> _frag13Pool = new List<GameObject> ();
	private List<GameObject> _frag23Pool = new List<GameObject> ();
	private List<GameObject> _frag33Pool = new List<GameObject> ();


	/// <summary>
	/// Method to spawn a new fragment of asteroid in game.
	/// </summary>
	/// <param name="position">Vector3 position to spawn fragment.</param>
	public void SpawnFragment(Vector3 position) {

		//random possibility to spawn fragent with 2 or 3 parts
		if (Random.Range (_RANGE_FROM, _RANGE_TO) < _FRAGMENT_PROPORTION) {
			
			NetworkServer.Spawn (GetFromPoolOrCreate (_frag12Pool, _frag12Prefab, _INITIAL_POSITION_FRAG_1_2 + position, Quaternion.identity));
			NetworkServer.Spawn (GetFromPoolOrCreate (_frag22Pool, _frag22Prefab, _INITIAL_POSITION_FRAG_2_2 + position, Quaternion.identity));
		} else {
			
			NetworkServer.Spawn (GetFromPoolOrCreate (_frag13Pool, _frag13Prefab, _INITIAL_POSITION_FRAG_1_3 + position, _INITIAL_ROTATION_FRAG_1_3));
			NetworkServer.Spawn (GetFromPoolOrCreate (_frag23Pool, _frag23Prefab, _INITIAL_POSITION_FRAG_2_3 + position, _INITIAL_ROTATION_FRAG_2_3));
			NetworkServer.Spawn (GetFromPoolOrCreate (_frag33Pool, _frag33Prefab, _INITIAL_POSITION_FRAG_3_3 + position, _INITIAL_ROTATION_FRAG_3_3));
		}
	}



	/// <summary>
	/// Method to get fragment from pool or create a new fragment if anyone available.
	/// </summary>
	/// <param name="objects">Pool list</param>
	/// <param name="prefab">Object prefab in case anyone available.</param>
	/// <param name="position">Vector3 position to spawn fragment.</param>
	/// <param name="rotation">Quaternion rotation to spawn fragment.</param>
	/// <returns>Return fragment available to use.</returns>
	GameObject GetFromPoolOrCreate (List<GameObject> objects, GameObject prefab, Vector3 position, Quaternion rotation) {

		foreach (GameObject obj in objects) {
			if (!obj.activeInHierarchy) {
				obj.transform.position = position;
				obj.transform.GetComponent<Rigidbody>().velocity = new Vector3 (
					Random.Range(-FRAGMENT_VELOCITY, FRAGMENT_VELOCITY) * 0.5f, 
					0f, 
					Random.Range(-FRAGMENT_VELOCITY, FRAGMENT_VELOCITY) * 0.5f);
				obj.transform.localScale = Vector3.one;
				obj.SetActive (true);
				return obj;
			}
		}

		//No fragment available
		GameObject newObject = (GameObject) Instantiate (prefab, position, rotation);
		newObject.name = "FragmentPoolObject" + objects.Count;
		newObject.SetActive(true);
		newObject.transform.GetComponent<Rigidbody>().velocity = new Vector3 (
			Random.Range(-FRAGMENT_VELOCITY, FRAGMENT_VELOCITY) * 0.5f, 
			0f, 
			Random.Range(-FRAGMENT_VELOCITY, FRAGMENT_VELOCITY) * 0.5f);
		
		newObject.transform.parent = this.gameObject.transform;
		objects.Add (newObject);
		return newObject;
	}

}
