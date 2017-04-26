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
	private bool isDead;
	public bool Dead {
		get { return isDead; }
		set { 
			isDead = value;
			if (isDead) {
				scoreManager.gameObject.SetActive (false);
				deadManager.SetActive (true, PlayerBehaviour.instance.GetHighScore (), score);
			} else {
				scoreManager.gameObject.SetActive (true);
				deadManager.SetActive (false, 0, 0);
			}
		}
	}

	public ParticleSystem rocket_PS;

	public ParticleSystem boost_PS;
	private bool boosted = false;

	public ParticleSystem exlosion_PS;

	private Rigidbody rb;
	BulletSpawnManagerBehaviour spawnManager;
	ScoreManagerBehaviour scoreManager;
	DeadManagerBehaviour deadManager;

	public Transform bulletPoint;

	void Start () {
		base.OnStartServer ();

		rb = GetComponent<Rigidbody> ();
		rb.position = transform.position;

		if (isLocalPlayer) {
			Dead = false;
		}

		if (isServer) {
			spawnManager = GameObject.Find ("BulletSpawnManager").GetComponent<BulletSpawnManagerBehaviour> ();
		}
			
	}

	void Update () {

		if (!isLocalPlayer || isDead)
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
		
	void FixedUpdate () {

		if (!isLocalPlayer || isDead)
			return;

		if (BoostButtonBehaviour.instance.IsPressed ()) {

			if (!boosted) {

				boost_PS.Play ();
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
		deadManager = GameObject.FindGameObjectWithTag ("DeadManager").GetComponent<DeadManagerBehaviour> ();
	}

	public override void OnStartClient () {
		base.OnStartClient ();

		gameObject.transform.name = "PLAYER" + gameObject.GetComponent<NetworkIdentity> ().netId.ToString ();

		if (!isServer)
			return;

		PlayersManager.instance.AddPlayer (gameObject.transform.name, this);
	}

	[Command]
	void CmdFire(Vector3 position, Quaternion rotation, string id) {

		GameObject obj = spawnManager.GetFromPool(position, rotation); 
		BulletBehaviour bullet = obj.GetComponent<BulletBehaviour> ();
		bullet.Fire (id);

		NetworkServer.Spawn(obj, spawnManager.assetId);
		StartCoroutine (bullet.Remove ());
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

	void OnCollisionEnter(Collision collision) { 

		Dead = true;

		rocket_PS.Stop ();
		//exlosion_PS.Play ();

	}
}
