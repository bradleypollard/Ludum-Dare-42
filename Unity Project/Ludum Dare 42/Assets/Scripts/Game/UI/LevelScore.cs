﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScore : MonoBehaviour
{
	public string LevelName = "";
	private GameplayManager Manager;
	public Text ScoreText;
    private Button LevelButton;

	// Use this for initialization
	void Start()
	{
		Manager = FindObjectOfType<GameplayManager>();
        LevelButton = GetComponent<Button>();

        LevelButton.onClick.AddListener( delegate { Manager.StartGame( LevelName ); } );
	}

	private void Update()
	{
        if (ScoreText != null)
        {
            int score = PlayerPrefs.GetInt(LevelName + "Score", -1);
            if (score >= 0)
            {
                ScoreText.text = score.ToString() + " Cells Left";
            }
            else
            {
                ScoreText.text = "";
            }
        }
	}
}
