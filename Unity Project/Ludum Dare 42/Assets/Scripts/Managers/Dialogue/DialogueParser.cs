using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using UnityEngine;
using RoboTools.Saving;

/// <summary>
/// V1.0 - DialogueParser
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Read's Script Files converting them to Dialogue
/// </summary>
namespace RoboTools
{
    namespace Dialogue
    {
        public class DialogueParser : MonoBehaviour
        {
            //Components
            private SaveDataManager saveDataManager;
            private CustomStringSavingManager customStringSavingManager;

            /// <summary>
            /// Load the a Dialogue Script from an XML File
            /// </summary>
            public DialogueInstance LoadDialogue(string _fileName)
            {
                TextAsset textAsset = Resources.Load<TextAsset>("Dialogue/" + _fileName);
                if (textAsset == null)
                {
                    throw new Exception("Dialogue file \"Dialogue/" + _fileName + "\" dosen't exist.");
                }

                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(textAsset.text);

                DialogueInstance dialogueInstance = new DialogueInstance();

                saveDataManager = FindObjectOfType<SaveDataManager>();
                customStringSavingManager = FindObjectOfType<CustomStringSavingManager>();

                float startTime = Time.realtimeSinceStartup;
                Debug.Log("Started loading " + _fileName);

                //Setup inital variables
                bool loadedCharacters = false, loadedDialogue = false;

                foreach (XmlNode xmlNode in xmldoc.DocumentElement.ChildNodes)
                {
                    //Load the Main Groups (i.e. Characters / Dialogue)
                    switch (xmlNode.Name)
                    {
                        case "Characters":
                            {
                                Debug.Log("Loading Group: " + xmlNode.Name);
                                if (!loadedCharacters)
                                {
                                    loadedCharacters = true;
                                    LoadCharacters(xmlNode, dialogueInstance);
                                }
                                else
                                {
                                    throw new Exception("Characters have already been loaded!");
                                }
                                break;
                            }
                        case "Dialogue":
                            {
                                Debug.Log("Loading Group: " + xmlNode.Name);
                                if (!loadedDialogue)
                                {
                                    loadedDialogue = true;
                                    LoadDialogue(xmlNode, dialogueInstance);
                                }
                                else
                                {
                                    throw new Exception("Dialogue has already been loaded!");
                                }
                                break;
                            }
                    }
                }

                Debug.Log("Finished loading " + _fileName + " in " + (Time.realtimeSinceStartup - startTime).ToString() + " seconds.");
                return dialogueInstance;
            }

            /// <summary>
            /// Load the Characters XML Node from the Dialogue Script
            /// </summary>
            public void LoadCharacters(XmlNode _node, DialogueInstance _dialogueInstance)
            {
                foreach (XmlNode characterNode in _node.ChildNodes)
                {
                    if (characterNode.Name == "DialogueCharacter")
                    {
                        //Load Values
                        string id = characterNode.Attributes["id"].Value;
                        CharacterItem characterItem = new CharacterItem(id);

                        if (customStringSavingManager != null)
                        {
                            characterItem.printName = customStringSavingManager.FixString(characterNode.Attributes["name"].Value);
                        }
                        else
                        {
                            characterItem.printName = characterNode.Attributes["name"].Value;
                        }

                        if (characterNode.Attributes["resource"] != null)
                            characterItem.prefabResource = characterNode.Attributes["resource"].Value;
                        if (characterNode.Attributes["gibber"] != null)
                            characterItem.gibberResource = characterNode.Attributes["gibber"].Value;

                        //Load Resources
                        if (characterItem.prefabResource != "")
                        {
                            characterItem.prefab = Resources.Load<GameObject>("Dialogue/CharacterPrefabs/" + characterItem.prefabResource);
                        }

                        if (characterItem.gibberResource != "")
                        {
                            characterItem.gibberNoise = Resources.Load<AudioClip>("Dialogue/SFX/" + characterItem.gibberResource);
                        }

                        _dialogueInstance.currentCharacters.Add(id, characterItem);

                        //Debug
                        string logPrint = "Loaded " + characterItem.printName;
                        logPrint += ": " + ((characterItem.prefab != null) ? characterItem.prefabResource : "{NO PREFAB}");
                        logPrint += ": " + ((characterItem.gibberNoise != null) ? characterItem.gibberResource : "{NO GIBBER}");

                        Debug.Log(logPrint);
                    }
                }
            }

            /// <summary>
            /// Load the Dialogue XML Node from the Dialogue Script
            /// </summary>
            public void LoadDialogue(XmlNode _node, DialogueInstance _dialogueInstance)
            {
                foreach (XmlNode dialogueNode in _node.ChildNodes)
                {
                    if (dialogueNode.Name == "Item")
                    {
                        if (!_dialogueInstance.currentDialogue.ContainsKey((_dialogueInstance.defaultDialogueCount + 1).ToString()))
                        {
                            //Load Speech
                            SpeechItem speechItem = new SpeechItem(_dialogueInstance.defaultDialogueCount++);
                            speechItem.characterID = dialogueNode.Attributes["speaker"].Value;

                            if (dialogueNode.Attributes["audio"] != null)
                            {
                                speechItem.audioPrefabName = dialogueNode.Attributes["audio"].Value;
                                speechItem.audioPrefab = Resources.Load<AudioClip>("Dialogue/SFX/" + speechItem.audioPrefabName);
                            }

                            if (dialogueNode.Attributes["music"] != null)
                            {
                                speechItem.musicClipName = dialogueNode.Attributes["music"].Value;
                                speechItem.musicClip = Resources.Load<AudioClip>("Dialogue/Music/" + speechItem.musicClipName);
                            }

                            if (dialogueNode.Attributes["speed"] != null)
                            {
                                speechItem.customSpeed = float.Parse(dialogueNode.Attributes["speed"].Value);
                            }
                            else
                            {
                                speechItem.customSpeed = 1f;
                            }

                            if (dialogueNode.Attributes["size"] != null)
                            {
                                speechItem.customSize = float.Parse(dialogueNode.Attributes["size"].Value);
                                //Debug.Log("Loading #" + id + ": Size Parameter " + speechItem.customSize);
                            }
                            else
                            {
                                speechItem.customSize = 1f;
                            }

                            if (dialogueNode.Attributes["requireinput"] != null)
                            {
                                speechItem.requiresInput = bool.Parse(dialogueNode.Attributes["requireinput"].Value);
                            }
                            else
                            {
                                speechItem.requiresInput = true;
                            }

                            if (dialogueNode.Attributes["animation"] != null)
                            {
                                string[] animationParamStrings = dialogueNode.Attributes["animation"].Value.Split(':');

                                string animationName = animationParamStrings[0];
                                List<object> parameters = new List<object>();

                                if (animationParamStrings.Length > 1)
                                {
                                    for (int j = 1; j < animationParamStrings.Length; j++)
                                    {
                                        string paramToParse = animationParamStrings[j];
                                        List<string> readIn = new List<string>();
                                        string current = "";

                                        for (int i = 0; i < paramToParse.Length; i++)
                                        {
                                            if (paramToParse[i] == '(' || paramToParse[i] == ')' || paramToParse[i] == ',')
                                            {
                                                readIn.Add(current);
                                                current = "";
                                            }
                                            else
                                            {
                                                current += paramToParse[i];
                                            }
                                        }

                                        switch (readIn[0])
                                        {
                                            case "int":
                                                parameters.Add(int.Parse(readIn[1]));
                                                break;
                                            case "float":
                                                parameters.Add(float.Parse(readIn[1]));
                                                break;
                                            case "vec2":
                                                parameters.Add(new Vector2(float.Parse(readIn[1]), float.Parse(readIn[2])));
                                                break;
                                            case "vec3":
                                                parameters.Add(new Vector3(float.Parse(readIn[1]), float.Parse(readIn[2]), float.Parse(readIn[3])));
                                                break;
                                            default:
                                                Debug.LogError("Dialogue #" + (_dialogueInstance.defaultDialogueCount + 1).ToString() + " has a broken animation param");
                                                continue;
                                        }
                                    }
                                }

                                speechItem.animationName = animationName;
                                speechItem.animationParams = parameters.Count > 0 ? parameters.ToArray() : null;
                            }
                            else
                            {
                                speechItem.animationName = null;
                            }

                            //Read in Text
                            if (dialogueNode.Attributes["text"] != null)
                            {
                                //TO DO
                                //- Run through Translation Manager

                                string originalText = dialogueNode.Attributes["text"].Value.Replace("\\n", "\n");

                                if (customStringSavingManager != null)
                                {
                                    originalText = customStringSavingManager.FixString(originalText);
                                }

                                string finalFormattedText = "";
                                string finalReadText = "";

                                List<Dictionary<Effect.EffectType, Effect>> effects = new List<Dictionary<Effect.EffectType, Effect>>();
                                Dictionary<Effect.EffectType, Effect> currentEffectStack = new Dictionary<Effect.EffectType, Effect>();

                                if (!ReadInString(originalText, 0, currentEffectStack, ref effects, ref finalFormattedText, ref finalReadText))
                                {
                                    Debug.LogError("Dialogue #" + (_dialogueInstance.defaultDialogueCount + 1).ToString());
                                    continue;
                                }

                                speechItem.skipText = false;
                                speechItem.finalFormattedText = finalFormattedText;
                                speechItem.finalReadText = finalReadText;

                                //Send Effects to Speech Item
                                speechItem.textEffects = effects;
                            }
                            else
                            {
                                speechItem.skipText = true;
                            }

                            _dialogueInstance.currentDialogue.Add(speechItem.id.ToString(), speechItem);
                        }
                        else
                        {
                            Debug.LogError("Dialogue #" + (_dialogueInstance.defaultDialogueCount + 1).ToString() + "has already been declared");
                        }
                    }
                }
            }

            private bool ReadInString(string _toRead, int _depth, Dictionary<Effect.EffectType, Effect> _currentEffectStack, ref List<Dictionary<Effect.EffectType, Effect>> _effects, ref string _formattedText, ref string _readText)
            {
                for (int i = 0; i < _toRead.Length; i++)
                {
                    //Check for Effect Character
                    if (_toRead[i] == '*')
                    {
                        //Check that effect is setup properly
                        if (_toRead[i + 1] == '(')
                        {
                            //We're reading in an effect, look for the end bracket!
                            int checkVal = i + 2;
                            while (checkVal < _toRead.Length)
                            {
                                if (_toRead[checkVal] == ')')
                                {
                                    break;
                                }

                                checkVal++;
                            }

                            if (checkVal >= _toRead.Length)
                            {
                                Debug.LogError(_formattedText + " has an effect with a )!");
                                return false;
                            }
                            else
                            {
                                Dictionary<Effect.EffectType, Effect> effectStack = new Dictionary<Effect.EffectType, Effect>(_currentEffectStack);
                                //We've got the effects!
                                if (!ReadEffects(_toRead.Substring(i + 2, checkVal - (i + 2)), ref effectStack))
                                {
                                    Debug.LogError(_formattedText + " has a bad parameter!");
                                    return false;
                                }

                                int endVal = checkVal + 1;
                                int starCount = 0;
                                while (endVal < _toRead.Length)
                                {
                                    if (_toRead[endVal] == '*' && (endVal + 1 >= _toRead.Length || _toRead[endVal + 1] != '('))
                                    {
                                        if (starCount > 0)
                                        {
                                            starCount--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    if (_toRead[endVal] == '*' && endVal <= _toRead.Length && _toRead[endVal + 1] == '(')
                                    {
                                        starCount++;
                                    }

                                    endVal++;
                                }

                                if (starCount != 0 || endVal >= _toRead.Length)
                                {
                                    Debug.LogError(_formattedText + " dosen't have a * to end an effect!");
                                    return false;
                                }
                                else
                                {
                                    int length = endVal - (checkVal + 1);
                                    if (length > 0)
                                    {
                                        //*(Effect)Text goes here*
                                        ReadInString(_toRead.Substring(checkVal + 1, length), _depth + 1, effectStack, ref _effects, ref _formattedText, ref _readText);
                                        i = endVal;
                                        continue;
                                    }
                                    else
                                    {
                                        //*(Effect)* - Add a * to show that there is an effect but no text
                                        _readText += "*";
                                        i = endVal;
                                        _effects.Add(effectStack);
                                        continue;
                                    }
                                }

                            }
                        }
                        else if (_toRead[i + 1] == '*')
                        {
                            _formattedText += "*";
                            _readText += " ";
                            i++;
                        }
                        else
                        {
                            //This Effect hasn't been setup corectely and is invalid
                            Debug.LogError(_formattedText + " has an unformatted effect!");
                            return false;
                        }
                    }
                    else
                    {
                        _formattedText += _toRead[i];
                        _readText += _toRead[i];
                    }

                    //Finally Add the effects list
                    {
                        _effects.Add(_currentEffectStack);
                    }
                }

                if (_depth == 0)
                {
                    Debug.Log("Formatted Text:" + _formattedText);
                    Debug.Log("Read Text: " + _readText);
                }

                return true;
            }

            private bool ReadEffects(string _eventParameters, ref Dictionary<Effect.EffectType, Effect> _events)
            {
                string[] parameters = _eventParameters.Split(',');
                foreach (string parameter in parameters)
                {
                    Effect newEffect = null;
                    string[] data = parameter.Split(':');

                    switch (data[0])
                    {
                        case "Size":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 2)
                                {
                                    return false;
                                }

                                //Read in Size Value, Cancel out if not Valid
                                int value;
                                if (!int.TryParse(data[1], out value))
                                {
                                    return false;
                                }

                                newEffect = new Effect(Effect.EffectType.Size, new IntData(value));
                            }
                            break;
                        case "Colour":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 2)
                                {
                                    return false;
                                }

                                newEffect = new Effect(Effect.EffectType.Colour, new StringData(data[1]));
                            }
                            break;
                        case "Font":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 2)
                                {
                                    return false;
                                }

                                //Make Font Effect                  
                                newEffect = new Effect(Effect.EffectType.Font, new StringData(data[1]));
                            }
                            break;
                        case "Bold":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 1)
                                {
                                    return false;
                                }

                                //Make Bold Effect                 
                                newEffect = new Effect(Effect.EffectType.Bold, new NullData());
                            }
                            break;
                        case "Italic":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 1)
                                {
                                    return false;
                                }

                                //Make Italic Effect                 
                                newEffect = new Effect(Effect.EffectType.Italic, new NullData());
                            }
                            break;
                        case "ShakeStart":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 1 || _events.ContainsKey(Effect.EffectType.ShakeStart) || _events.ContainsKey(Effect.EffectType.ShakeEnd))
                                {
                                    return false;
                                }

                                //Make ShakeStart Effect                 
                                newEffect = new Effect(Effect.EffectType.ShakeStart, new NullData());
                            }
                            break;
                        case "ShakeEnd":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 1 || _events.ContainsKey(Effect.EffectType.ShakeStart) || _events.ContainsKey(Effect.EffectType.ShakeEnd))
                                {
                                    return false;
                                }

                                //Make Italic Effect                 
                                newEffect = new Effect(Effect.EffectType.ShakeEnd, new NullData());
                            }
                            break;
                        case "Pause":
                            {
                                //Cancel Out if Paramter not set up correctely
                                if (data.Length != 2)
                                {
                                    return false;
                                }

                                //Read in Pause Value, Cancel out if not Valid
                                float value;
                                if (!float.TryParse(data[1], out value))
                                {
                                    return false;
                                }

                                newEffect = new Effect(Effect.EffectType.Pause, new FloatData(value));
                            }
                            break;
                        default:
                            return false;
                    }

                    //Check if Effect already Exists
                    if (_events.ContainsKey(newEffect.effectType))
                    {
                        _events[newEffect.effectType] = newEffect;
                    }
                    else
                    {
                        _events.Add(newEffect.effectType, newEffect);
                    }
                }

                return true;
            }
        }
    }
}
