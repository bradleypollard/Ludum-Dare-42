using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoboTools.Saving;

public class Test : MonoBehaviour
{
    string testText;

    //Components
    CustomStringSavingManager customStringSavingManager;
    SaveDataManager saveDataManager;


    private void Start()
    {
        customStringSavingManager = FindObjectOfType<CustomStringSavingManager>();
        saveDataManager = FindObjectOfType<SaveDataManager>();
    }

    public void OnGUI()
    {
        testText = GUI.TextField(new Rect(10, 10, 300, 50), testText);

        if(GUI.Button(new Rect(10, 70, 300, 100), "Save"))
        {
            customStringSavingManager.SetCustomString("Test", testText);
            saveDataManager.Save("Test");
        }

        if (GUI.Button(new Rect(10, 180, 300, 100), "Load"))
        {
            saveDataManager.Load("Test");
            testText = customStringSavingManager.GetCustomString("Test");
        }
    }
}
