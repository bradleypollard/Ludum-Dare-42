using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// V1.0 - TextConverter
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Uses the Translation Manager to convert the text inside of a Unity Text Component
/// </summary>
namespace RoboTools
{
    namespace Translation
    {
        [RequireComponent(typeof(Text)), ExecuteInEditMode]
        public class TextConverter : MonoBehaviour
        {
            public string boxString;
            private TranslationManager translationManager;

            private void Awake()
            {
                translationManager = FindObjectOfType<TranslationManager>();
                GetComponent<Text>().text = translationManager.GetString(boxString);
            }
        }
    }
}
