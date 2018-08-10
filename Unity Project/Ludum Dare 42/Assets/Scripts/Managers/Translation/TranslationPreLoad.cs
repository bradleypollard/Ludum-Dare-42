using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - TranslationPreLoad
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Preloads the specified files into the Translation Manager
/// </summary>
namespace RoboTools
{
    namespace Translation
    {
        [ExecuteInEditMode]
        public class TranslationPreLoad : MonoBehaviour
        {
            public string[] requiredFiles;
            private TranslationManager translationManager;

            // Use this for initialization
            void Awake()
            {
                translationManager = FindObjectOfType<TranslationManager>();

                foreach (string path in requiredFiles)
                {
                    translationManager.LoadDialogueXML(path);
                }
            }
        }
    }
}
