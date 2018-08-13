using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButtonsGenerator : MonoBehaviour
{
    public GameObject levelButtonPrefab, levelButtonLockedPrefab, levelButtonCompletePrefab, scrollView;
    public List<string> levelNames;
    private List<GameObject> levelObjects;

    private void Start()
    {
        RegenerateLevels();
    }

    public void RegenerateLevels()
    {
        //Delete old GameObjects
        if(levelObjects != null)
        {
            foreach(GameObject menuObject in levelObjects)
            {
                Destroy(menuObject);
            }
        }

        //Get Unlocked Levels
        int isUnlocked = 2;
        levelObjects = new List<GameObject>();

        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(222 * levelNames.Count, 400);
        scrollView.GetComponent<RectTransform>().offsetMax = new Vector2(-(1500 - 300 - (222 * levelNames.Count)), scrollView.GetComponent<RectTransform>().offsetMax.y); // new Vector2(-right, -top);

        for (int i = 0; i < levelNames.Count; i++)
        {
            GameObject levelButton = null;

            int completedLevel = PlayerPrefs.GetInt(levelNames[i] + "Complete", 0);
            if (completedLevel == 1)
            {
                levelButton = Instantiate(levelButtonCompletePrefab, scrollView.transform);
            }
            else if (isUnlocked > 0)
            {
                levelButton = Instantiate(levelButtonPrefab, scrollView.transform);
                isUnlocked --;
            }
            else
            {
                levelButton = Instantiate(levelButtonLockedPrefab, scrollView.transform);
            }
            levelObjects.Add(levelButton);

            RectTransform rect = levelButton.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(150 + (222 * i), 200);

            LevelScore levelScore = levelButton.GetComponent<LevelScore>();
            if (levelScore != null)
            {
                levelScore.LevelName = levelNames[i];
            }

            Text levelText = levelButton.transform.Find("LevelName").GetComponent<Text>();

            if (i > 0)
            {
                levelText.text = "Level" + System.Environment.NewLine + (i).ToString();
            }
            else
            {
                levelText.text = "Infinite";
                //rect.Rotate(new Vector3(0, 0, 90));
            }
        }
    }
}
