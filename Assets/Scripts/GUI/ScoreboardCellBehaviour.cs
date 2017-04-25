using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardCellBehaviour : MonoBehaviour {

	public Text m_Position;
	public Image m_Picture;
	public Text m_UserName;
	public Text m_Level;
	public Text m_K;
	public Text m_D;
	public Text m_HighScore;

	public void SetValues (int position, string name, int level, int k, int d, int highScore) {

		if (position <= 9) {
			m_Position.text = "0" + position + ".";
		} else {
			m_Position.text = position + ".";
		}

		m_UserName.text = name;

		m_K.text = ""+k;
		m_D.text = ""+d;

		m_HighScore.text = ""+highScore;


	}





}
