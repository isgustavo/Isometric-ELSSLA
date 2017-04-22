using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : NetworkBehaviour {

	private const int INITIAL_SCORE = 0;

	private int speed = 10;
	private float timeTilNextShot = .0f;
	private float timeBetweenShot = .3f;

	[SyncVar (hook="OnScoreChange")]
	public int score = INITIAL_SCORE;

	public ParticleSystem boostExplosion;
	private bool boosted = false;

	private Rigidbody rb;
	BulletSpawnManagerBehaviour spawnManager;
	ScoreManagerBehaviour scoreManager;

	public Transform bulletPoint;

	void Start () {

		rb = GetComponent<Rigidbody> ();
		rb.position = transform.position;

		if (isServer) {
			spawnManager = GameObject.Find ("BulletSpawnManager").GetComponent<BulletSpawnManagerBehaviour> ();
		}
			
	}

	void Update () {

		if (!isLocalPlayer)
			return;

		if (RotationJoystickBehaviour.instance.IsDragging ()) {
			gameObject.transform.rotation = Quaternion.AngleAxis (RotationJoystickBehaviour.instance.GetAngle (), Vector3.up);

			if (timeTilNextShot < 0) {
				timeTilNextShot = timeBetweenShot;

				CmdFire(bulletPoint.position, bulletPoint.rotation, gameObject.transform.name);
			}
		}

		timeTilNextShot -= Time.deltaTime;

	}

	[Command]
	void CmdFire(Vector3 position, Quaternion rotation, string id) {

		GameObject obj = spawnManager.GetFromPool(position, rotation); 
		BulletBehaviour bullet = obj.GetComponent<BulletBehaviour> ();
		bullet.Fire (id);

		NetworkServer.Spawn(obj, spawnManager.assetId);
		StartCoroutine (bullet.Remove ());
	}

		
	void FixedUpdate () {

		if (!isLocalPlayer)
			return;

		if (BoostButtonBehaviour.instance.IsPressed ()) {

			if (!boosted) {

				boostExplosion.Play ();
				boosted = true;

				rb.AddForce (transform.forward * speed * 10f, ForceMode.Acceleration);
			} else {

				rb.AddForce (transform.forward * speed, ForceMode.Acceleration);
			}
				
		} else {

			boosted = false;
		}
	}

	public override void OnStartLocalPlayer () {
		base.OnStartServer ();

		scoreManager = GameObject.FindGameObjectWithTag ("ScoreManager").GetComponent<ScoreManagerBehaviour> ();

	}

	public override void OnStartClient () {
		base.OnStartClient ();

		gameObject.transform.name = "PLAYER" + gameObject.GetComponent<NetworkIdentity> ().netId.ToString ();

		if (!isServer)
			return;

		PlayersManager.instance.AddPlayer (gameObject.transform.name, this);
	}

	public void OnScoreChange (int value) {

		score = value;
		if (value > PlayerBehaviour.instance.GetHighScore ()) {
			//highscore = value;
		}
			
		if (!isLocalPlayer)
			return;

		scoreManager.m_Score = score;

	}
}
