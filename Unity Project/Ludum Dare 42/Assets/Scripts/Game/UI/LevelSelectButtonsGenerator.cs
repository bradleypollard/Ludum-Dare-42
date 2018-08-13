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
        bool isUnlocked = true;
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
            else if (isUnlocked)
            {
                levelButton = Instantiate(levelButtonPrefab, scrollView.transform);
                isUnlocked = false;
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
            levelText.text = "Level" + System.Environment.NewLine + (i + 1).ToString();
        }
    }
}
