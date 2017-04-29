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

	[SyncVar]
	public string name;

	[SyncVar (hook="OnScoreChange")]
	public int score = INITIAL_SCORE;

	public ParticleSystem rocket_PS;

	public ParticleSystem boost_PS;
	private bool boosted = false;

	public ParticleSystem exlosion_PS;

	private Rigidbody rb;
	BulletSpawnManagerBehaviour spawnManager;
	ScoreManagerBehaviour scoreManager;
	DeadManagerBehaviour deadManager;

	public Transform bulletPoint;

	private GameSceneBehaviour _gameSceneManager;

	void Start () {
		base.OnStartServer ();

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
		
	void FixedUpdate () {

		if (!isLocalPlayer)
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
		//deadManager = GameObject.FindGameObjectWithTag ("DeadManager").GetComponent<DeadManagerBehaviour> ();

		GameObject obj = GameObject.FindGameObjectWithTag ("GameScene");
		if (obj != null) {

			_gameSceneManager = obj.GetComponent<GameSceneBehaviour> ();

		}

		PlayerBehaviour.instance.observer = _gameSceneManager.deadScene.GetComponent<DeadSceneBehaviour> ();
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

		/*score = value;
		if (value > PlayerBehaviour.instance.GetHighScore ()) {
			//highscore = value;
		}
			
		if (!isLocalPlayer)
			return;

		scoreManager.m_Score = score;*/

	}

	void OnCollisionEnter(Collision collision) { 

		//rocket_PS.Stop ();
		_gameSceneManager.SetState(GameSceneBehaviour.State.Dead);
		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
		/*	PlayerBehaviour.instance.SaveKD (bullet.id);
			ControllerBehaviour player = PlayersManager.instance.GetPlayer (bullet.id);

			if (player != null) {


				deadManager.SetActive (true, PlayerBehaviour.instance.GetHighScore (), score, player.name, PlayerBehaviour.instance.GetKD(bullet.id + ""));
			}
*/
		}


		//exlosion_PS.Play ();

	}
}
