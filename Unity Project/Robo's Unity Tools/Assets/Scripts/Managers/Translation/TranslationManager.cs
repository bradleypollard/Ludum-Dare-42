using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

/// <summary>
/// V1.0 - TranslationManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Manages Game Translation
/// </summary>
namespace RoboTools
{
    namespace Translation
    {
        [ExecuteInEditMode]
        public class TranslationManager : MonoBehaviour
        {
            public string defaultLanguage = "English";
            private string currentLanguage;

            private List<string> loadedResourcesPaths;
            private Dictionary<string, string> loadedText;

            private readonly List<string> validLanguages = new List<string>() { "English", "Pirate" };

            // Use this for initialization
            void Awake()
            {
                //Load the Default Language
                LoadLanguage(defaultLanguage);
            }

            public void LoadLanguage(string _newLanguage)
            {
                if (!validLanguages.Contains(_newLanguage))
                {
                    Debug.LogError(_newLanguage + " is not a valid language!");
                    return;
                }

                currentLanguage = _newLanguage;

                //Create new Dictionary
                loadedText = new Dictionary<string, string>();

                //Reload Loaded Text
                if (loadedResourcesPaths != null)
                {
                    foreach (string path in loadedResourcesPaths)
                    {
                        LoadDialogueXML(path);
                    }
                }
                else
                {
                    loadedResourcesPaths = new List<string>();
                }

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="path">Path from Resources/Localisation/{Language}/</param>
            public void LoadDialogueXML(string path)
            {
                if (!loadedResourcesPaths.Contains(path))
                {
                    string finalPath = "Localisation/" + currentLanguage + "/" + path;

                    TextAsset textAsset = Resources.Load<TextAsset>(finalPath);
                    if (textAsset == null)
                    {
                        Debug.LogError(finalPath + " does not exist!");
                        return;
                    }

                    //Start Loading the Text
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.LoadXml(textAsset.text);

                    Dictionary<string, string> localChanges = new Dictionary<string, string>();

                    foreach (XmlNode xmlNode in xmldoc.DocumentElement.ChildNodes)
                    {
                        if (xmlNode.Name == "Item")
                        {
                            if (xmlNode.Attributes["id"] == null || xmlNode.Attributes["text"] == null)
                            {
                                Debug.LogError(finalPath + " has an XML Node that isn't complete!");
                                return;
                            }

                            if (loadedText.ContainsKey(xmlNode.Attributes["id"].Value) || localChanges.ContainsKey(xmlNode.Attributes["id"].Value))
                            {
                                Debug.LogError(xmlNode.Attributes["id"].Value + " already exists");
                                return;
                            }

                            localChanges.Add(xmlNode.Attributes["id"].Value, xmlNode.Attributes["text"].Value);
                        }
                        else
                        {
                            Debug.LogError(finalPath + " has an XML Node that isn't an item!");
                            return;
                        }
                    }

                    //If we get here then all the text was loaded properly so is ready to be added to the actual loaded data
                    loadedResourcesPaths.Add(path);
                    foreach (KeyValuePair<string, string> pair in localChanges)
                    {
                        loadedText.Add(pair.Key, pair.Value);
                    }

                    Debug.Log("Loaded " + finalPath + " : " + localChanges.Keys.Count + " items");
                }
            }

            public string GetString(string _textID)
            {
                if (loadedText != null && loadedText.ContainsKey(_textID))
                {
                    return loadedText[_textID];
                }
                else
                {
                    return _textID;
                }
            }
        }
    }
}
