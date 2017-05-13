using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class DeadSceneBehaviour : MonoBehaviour {

		
	[SerializeField]
	private ScoreBehaviour _scoreContainer;

	[SerializeField]
	private Text _nameText;
	[SerializeField]
	private GameObject _killContent;
	[SerializeField]
	private GameObject _respawnText;
	[SerializeField]
	private GameObject _vendettaText;

	[SerializeField]
	private Text _coinCountText;
	[SerializeField]
	private GameObject _coinsContent;

	[SerializeField]
	private ScoreboardBehaviour _scoreboardContent;


	/// <summary>
	/// Sets the dead scene active.
	/// </summary>
	/// <param name="scored">Last players score.</param>
	/// <param name="name">If player destoryed by other player. Set the name</param>
	public void SetActive (int scored, string name) {

		gameObject.SetActive (true);
		_scoreContainer.InitWithScore (scored);

		if (name != "") {

			_killContent.SetActive (true);
			_nameText.text = name;
			_vendettaText.SetActive (true);
			_respawnText.SetActive (false);
		} else {
			_killContent.SetActive (false);
			_vendettaText.SetActive (false);
			_respawnText.SetActive (true);
		}
			
		_coinsContent.SetActive (true);
		_coinCountText.text = PlayerBehaviour.instance.localPlayer._coins.count.ToString ();

		_scoreboardContent.SetActive ();

	}


}
