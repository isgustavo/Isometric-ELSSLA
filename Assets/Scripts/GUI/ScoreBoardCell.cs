using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardCell : MonoBehaviour {

	[SerializeField]
	private Text _positionText;
	[SerializeField]
	private Image _playerImage;
	[SerializeField]
	private Text _nameText;
	[SerializeField]
	private Text _kText;
	[SerializeField]
	private Text _dText;
	[SerializeField]
	private Text _highScoreText;

	private Animator _animator;

	public void SetValues (int position, Sprite image, string name, int k, int d, int highScore) {
		Debug.Log ("position"+position);
		_positionText.text = position.ToString ();
		_playerImage.sprite = image;
		_nameText.text = name;
		_kText.text = k.ToString ();
		_dText.text = d.ToString ();
		_highScoreText.text = highScore.ToString ();

	}

	public void Show () {

		if (_animator == null) {

			_animator = gameObject.GetComponent<Animator> ();
		}

		_animator.Play ("Entry");
	}

}
