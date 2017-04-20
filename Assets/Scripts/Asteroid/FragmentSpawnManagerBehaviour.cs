using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FragmentSpawnManagerBehaviour : MonoBehaviour {

	public GameObject m_Frag12Prefab;
	private Vector3 m_initialPositionFrag21 = new Vector3(-.25f, 0f, 0f);
	public GameObject m_Frag22Prefab;
	private Vector3 m_initialPositionFrag22 = new Vector3(.25f, 0f, 0f);

	public GameObject m_Frag31Prefab;
	private Vector3 m_initialPositionFrag31 = new Vector3(0f, .35f, .093f);
	private Quaternion m_initialRotationFrag31 = new Quaternion (37, 0, 0, 0);
	public GameObject m_Frag32Prefab;
	private Vector3 m_initialPositionFrag32 = new Vector3(0f, 0f, 0f);
	private Quaternion m_initialRotationFrag32 = new Quaternion (60, 0, 0, 0);
	public GameObject m_Frag33Prefab;
	private Vector3 m_initialPositionFrag33 = new Vector3(0f, -.058f, -.309f);
	private Quaternion m_initialRotationFrag33 = new Quaternion (62, 0, 0, 0);

	public List<GameObject> m_Fragment12Pool = new List<GameObject> ();
	public List<GameObject> m_Fragment22Pool = new List<GameObject> ();

	public List<GameObject> m_Fragment31Pool = new List<GameObject> ();
	public List<GameObject> m_Fragment32Pool = new List<GameObject> ();
	public List<GameObject> m_Fragment33Pool = new List<GameObject> ();



	public void GetFromPool(Vector3 position) {

		if (Random.Range (0, 10) < 6) {
			
			ReSpawnFromPoolOrCreate (m_Fragment12Pool, m_Frag12Prefab, m_initialPositionFrag21 + position, Quaternion.identity);
			ReSpawnFromPoolOrCreate (m_Fragment22Pool, m_Frag22Prefab, m_initialPositionFrag22 + position, Quaternion.identity);
		} else {
			
			ReSpawnFromPoolOrCreate (m_Fragment31Pool, m_Frag31Prefab, m_initialPositionFrag31 + position, m_initialRotationFrag31);
			ReSpawnFromPoolOrCreate (m_Fragment32Pool, m_Frag32Prefab, m_initialPositionFrag32 + position, m_initialRotationFrag32);
			ReSpawnFromPoolOrCreate (m_Fragment33Pool, m_Frag33Prefab, m_initialPositionFrag33 + position, m_initialRotationFrag33);

		}
	}


	private void ReSpawnFromPoolOrCreate (List<GameObject> objects, GameObject prefab, Vector3 position, Quaternion rotation) {

		foreach (GameObject obj in objects) {
			if (!obj.activeInHierarchy) {
				obj.transform.position = position;
				obj.SetActive (true);
				NetworkServer.Spawn (obj);
				break;
			}
		}
			
		GameObject part = (GameObject) Instantiate (prefab, position, rotation);
		part.name = "FragmentPoolObject" + objects.Count;
		part.SetActive(true);
		part.transform.parent = this.gameObject.transform;

		objects.Add (part);
		NetworkServer.Spawn (part);
	}

}
