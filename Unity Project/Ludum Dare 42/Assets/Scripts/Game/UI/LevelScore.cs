using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScore : MonoBehaviour
{
	public string LevelName = "";
	private GameplayManager Manager;
	public Text ScoreText;
    private Button LevelButton;
    private RectTransform rectTransform;
    private Image image;

    // Use this for initialization
    void Start()
	{
		Manager = FindObjectOfType<GameplayManager>();
        LevelButton = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        LevelButton.onClick.AddListener( delegate { Manager.StartGame( LevelName ); } );
	}

	private void Update()
	{
        if (ScoreText != null)
        {
            if (LevelName != "")
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
            else
            {
                int score = PlayerPrefs.GetInt("InfiniteScore", 0);
                if (score >= 0)
                {
                    ScoreText.text = score.ToString() + " Waves";
                }
                else
                {
                    ScoreText.text = "";
                }
            }
        }
	}
}
