using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScore : MonoBehaviour
{
	public string LevelName = "";
	public GameplayManager Manager;
	public Text ScoreText;
	public Button LevelButton;

	// Use this for initialization
	void Start()
	{
		Manager = FindObjectOfType<GameplayManager>();

		LevelButton.onClick.AddListener( delegate { Manager.StartGame( LevelName ); } );
	}

	private void Update()
	{
		int score = PlayerPrefs.GetInt( LevelName + "Score", -1 );
		if ( score >= 0 )
		{
			ScoreText.text = score.ToString();
		}
		else
		{
			ScoreText.text = "";
		}
	}
}
