using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardCellBehaviour : MonoBehaviour {

	[SerializeField]
	private Text _positionText;
	[SerializeField]
	private Image _pictureImage;
	[SerializeField]
	private Text _nameText;
	[SerializeField]
	private Text _kText;
	[SerializeField]
	private Text _dText;
	[SerializeField]
	private Text _highScoreText;

	/// <summary>
	/// Sets the scoreboard cell values.
	/// </summary>
	/// <param name="position">Player position.</param>
	/// <param name="image">Facebook picture.</param>
	/// <param name="name">Players name.</param>
	/// <param name="kd">Players Kd.</param>
	/// <param name="highScore">Players High score.</param>
	public void SetValues (int position, Sprite image, string name, KD kd, int highScore) {
		transform.localScale = Vector3.one;

		_positionText.text = position.ToString ("00");
		_pictureImage.sprite = image;
		_nameText.text = name;

		if (kd != null) {

			_kText.text = kd._k.ToString ();
			_dText.text = kd._d.ToString ();
		}
			
		_highScoreText.text = highScore.ToString ();

	}
		

}
