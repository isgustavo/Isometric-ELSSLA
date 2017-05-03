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
	public int _score = INITIAL_SCORE;
	private bool _isNewHighScore = false;
	private bool _isDead = false;

	public ParticleSystem rocket_PS;
	public ParticleSystem boost_PS;
	private bool boosted = false;

	public ParticleSystem exlosion_PS;

	[SerializeField]
	private GameObject _mesh;
	private Rigidbody rb;
	private BoxCollider cc;
	BulletSpawnManagerBehaviour spawnManager;

	public Transform bulletPoint;

	private GameSceneBehaviour _gameSceneManager;

	void Start () {

		rb = GetComponent<Rigidbody> ();
		rb.position = transform.position;
		cc = GetComponent<BoxCollider> ();

		if (isServer) {
			spawnManager = GameObject.Find ("BulletSpawnManager").GetComponent<BulletSpawnManagerBehaviour> ();
		}
			
	}

	void Update () {

		if (!isLocalPlayer || _isDead)
			return;

		if (RotationJoystickBehaviour.instance.IsDragging ()) {
			gameObject.transform.rotation = Quaternion.AngleAxis (RotationJoystickBehaviour.instance.GetAngle (), Vector3.up);

			if (timeTilNextShot < 0) {
				timeTilNextShot = timeBetweenShot;

				CmdFire(bulletPoint.position, bulletPoint.rotation, PlayerBehaviour.instance.player.id, PlayerBehaviour.instance.player.name);
			}
		}

		timeTilNextShot -= Time.deltaTime;

	}
		
	void FixedUpdate () {

		if (!isLocalPlayer || _isDead)
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

		PlayerBehaviour.instance.UseCoin ();

		GameObject obj = GameObject.FindGameObjectWithTag ("GameScene");
		if (obj != null) {

			_gameSceneManager = obj.GetComponent<GameSceneBehaviour> ();
			_gameSceneManager._delegate += new RespawnDelegate (this.Respawn);
		}
	}

	public override void OnStartClient () {
		base.OnStartClient ();

		gameObject.transform.name = PlayerBehaviour.instance.player.id;

		if (!isServer)
			return;

		PlayersManager.instance.AddPlayer (gameObject.transform.name, this);
	}

	[Command]
	void CmdFire(Vector3 position, Quaternion rotation, string id, string name) {

		GameObject obj = spawnManager.GetFromPool(); 
		BulletBehaviour bullet = obj.GetComponent<BulletBehaviour> ();
		bullet.Fire (id, name, position, rotation);

		NetworkServer.Spawn(obj, spawnManager.assetId);
	}


	public void OnScoreChange (int value) {

		_score = value;
		if (value > PlayerBehaviour.instance.player.highScore) {
			_isNewHighScore = true;
		} else {
			_isNewHighScore = false;
		}
			
		if (!isLocalPlayer)
			return;
		
		_gameSceneManager.SetScore(_score, _isNewHighScore);

	}

	public void Respawn () {

		_score = 0;
		_mesh.SetActive (true);
		_isDead = false;
	}

	void OnCollisionEnter(Collision collision) { 

		Debug.Log (collision.gameObject.transform+""+collision.gameObject.activeInHierarchy);

		string name = "";
		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {
			//Debug.Log ("bullet.id"+bullet.id + "....PlayerBehaviour.instance.player.id" + PlayerBehaviour.instance.player.id);
			if (bullet.id == PlayerBehaviour.instance.player.id) {
				return;
			}

			PlayerBehaviour.instance.NewKD (bullet.id);
			name = bullet.playerName;
		}
			
		_isDead = true;
		_mesh.SetActive (false);
		rb.angularVelocity = Vector3.zero; 

		rocket_PS.Stop ();
		exlosion_PS.Play ();

		if (_isNewHighScore) {

			PlayerBehaviour.instance.NewHighScore (_score);
		}

		_gameSceneManager.Dead(_score, _isNewHighScore, name);

	}
}
