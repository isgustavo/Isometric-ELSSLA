using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public abstract class RespawnObserver: NetworkBehaviour  {

	public abstract void OnNotify ();
}

[RequireComponent(typeof(Rigidbody))]
public class ControllerBehaviour : RespawnObserver {

	private const int _INITIAL_SCORE = 0;
	private const int _SPEED = 10;
	private const int _BOOST_SPEED = 100;
	private const float _TIME_BETWEEN_SHOT = .3f;

	private float _timeTilNextShot = .0f;

	[SyncVar (hook="OnScoreChange")]
	public int _score = _INITIAL_SCORE;
	private bool _isDead = false;

	[SerializeField]
	private GameObject _mesh;
	[SerializeField]
	private Transform bulletPoint;
	[SerializeField]
	private ParticleSystem _rocket;
	[SerializeField]
	private ParticleSystem _boost;
	private bool _boosted = false;
	[SerializeField]
	private ParticleSystem _exlosion;

	[SerializeField]
	private Rigidbody _rb;

	private ScoreObserver _scoreObserver;
	private DeadObserver _deadObserver;

	//Just on server
	private BulletSpawnManagerBehaviour _spawnManager;


	void Start () {

		_rb.position = transform.position;
			
	}

	void Update () {

		if (!isLocalPlayer || _isDead)
			return;

		if (RotationJoystickBehaviour.instance.isDragging) {
			gameObject.transform.rotation = Quaternion.AngleAxis (RotationJoystickBehaviour.instance.angle, Vector3.up);

			if (_timeTilNextShot < 0) {
				_timeTilNextShot = _TIME_BETWEEN_SHOT;

				CmdFire(bulletPoint.position, bulletPoint.rotation, PlayerBehaviour.instance.localPlayer._id, PlayerBehaviour.instance.localPlayer._name);
			}
		}

		_timeTilNextShot -= Time.deltaTime;

	}
		
	void FixedUpdate () {

		if (!isLocalPlayer || _isDead)
			return;

		if (BoostButtonBehaviour.instance.pressed) {

			if (!_boosted) {

				_boost.Play ();
				_boosted = true;

				_rb.AddForce (transform.forward * _BOOST_SPEED, ForceMode.Acceleration);
			} else {

				_rb.AddForce (transform.forward * _SPEED, ForceMode.Acceleration);
			}
		} else {

			_boosted = false;
		}
	}


	/// <summary>
	/// Method called when the local player was stated.
	/// </summary>
	public override void OnStartLocalPlayer () {
		base.OnStartServer ();

		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<GameCameraBehaviour> ().SetTarget (transform);


		GameObject obj = GameObject.FindGameObjectWithTag ("GameScene");
		if (obj != null) {

			_scoreObserver = obj.GetComponentInChildren<ScoreObserver> ();
			_deadObserver = obj.GetComponent<DeadObserver> ();
			obj.GetComponent<GameSceneBehaviour> ()._observer = this;
		}
	}

	/// <summary>
	/// Method called when a client was stated.
	/// </summary>
	public override void OnStartClient () {
		base.OnStartClient ();

		gameObject.transform.name = PlayerBehaviour.instance.localPlayer._id;

		if (isServer) {

			PlayersManager.instance.AddPlayer (gameObject.transform.name, this);
		}
	}

	/// <summary>
	/// Method called when a server was stated.
	/// </summary>
	public override void OnStartServer () {
		base.OnStartServer ();

		_spawnManager = GameObject.Find ("BulletSpawnManager").GetComponent<BulletSpawnManagerBehaviour> ();
	}

	/// <summary>
	/// Server-side method to spawn a bullet fire.
	/// </summary>
	/// <param name="position">Start position.</param>
	/// <param name="rotation">Start rotation.</param>
	/// <param name="id">Player id.</param>
	/// <param name="name">Player name.</param>
	[Command]
	void CmdFire(Vector3 position, Quaternion rotation, string id, string name) {

		GameObject obj = _spawnManager.GetFromPool(); 
		BulletBehaviour bullet = obj.GetComponent<BulletBehaviour> ();
		bullet.Fire (id, name, position, rotation);

		NetworkServer.Spawn(obj, _spawnManager.assetId);
	}

	/// <summary>
	/// Hook method to change score value.
	/// Being a local player this hook going notify score display observer
	/// </summary>
	public void OnScoreChange (int value) {

		_score = value;

		if (isLocalPlayer) {

			_scoreObserver.OnNotify (_score);
		}
			
	}

	/// <summary>
	/// Player collider:
	/// Being a bullet the <param collision> going to get the players id and name from bullet  
	/// and send a request to update the KD value between local player and the player who destroyed 
	/// the local player.
	/// Being score bigger than last high score going to send a request to update the high score.
	/// At last, the _score, _isNewHighScore and local variable name going to send to game scene manager.
	/// </summary>
	void OnCollisionEnter(Collision collision) { 

		string name = "";
		BulletBehaviour bullet = collision.gameObject.GetComponent<BulletBehaviour> ();
		if (bullet != null) {

			PlayerBehaviour.instance.SaveNewKD (bullet.id);
			name = bullet.playerName;
		}

		_isDead = true;
		_mesh.SetActive (false);

		_rocket.Stop ();
		_exlosion.Play ();

		PlayerBehaviour.instance.SaveNewHighScore (_score);
		_deadObserver.OnNotify (_score, name);

	}

	/// <summary>
	/// Respawn notify
	/// </summary>
	public override void OnNotify () {
		Respawn ();
	}

	/// <summary>
	/// Method to set player with initial status.
	/// </summary>
	void Respawn () {

		_score = _INITIAL_SCORE;
		_mesh.SetActive (true);
		_rb.angularVelocity = Vector3.zero; 
		_rocket.Play ();

		_isDead = false;
	}


	/// <summary>
	/// Changeds the identifier.
	/// </summary>
	public void ChangedId () {

		string newId = PlayerBehaviour.instance.localPlayer._id;
		gameObject.transform.name = newId;

		CmdChangedId (newId);
	}


	/// <summary>
	/// Server-side method to notify Id changes.
	/// </summary>
	/// <param name="id">New player Id.</param>
	[Command]
	public void CmdChangedId (string id) {

		PlayersManager.instance.AddPlayer (id, this);
	}

}
