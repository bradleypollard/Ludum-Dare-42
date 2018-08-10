using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// V1.0 - CustomStringSavingManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Allows the saving of custom strings
/// </summary>
namespace RoboTools
{
    namespace Saving
    {
        public class CustomStringSavingManager : ISavingManager
        {
            public Dictionary<string, string> customStringDatabase;

            public string[] testStrings;

            private void Start()
            {
                //Setup
                customStringDatabase = new Dictionary<string, string>();

                //Load Debug Strings
                for (int i = 0; i < testStrings.Length; i += 2)
                {
                    customStringDatabase[testStrings[i]] = testStrings[i + 1];
                }
            }

            // ----------------------------- Getter / Setter Methods --------------------------------------
            public string GetCustomString(string _id)
            {
                if (customStringDatabase.ContainsKey(_id))
                {
                    return customStringDatabase[_id];
                }
                else
                {
                    return "";
                }
            }

            public void SetCustomString(string _id, string _value)
            {
                if (customStringDatabase.ContainsKey(_id))
                {
                    customStringDatabase[_id] = _value;
                }
                else
                {
                    customStringDatabase.Add(_id, _value);
                }
            }

            public string FixString(string _original)
            {
                foreach (KeyValuePair<string, string> pair in customStringDatabase)
                {
                    string toTest = "[" + pair.Key + "]";

                    while (_original.Contains(toTest))
                    {
                        _original = _original.Replace(toTest, pair.Value);
                    }
                }

                return _original;
            }

            // ----------------------------- Saving Methods --------------------------------------

            public override void DoSave(BinaryWriter _stream)
            {
                //Save the number of strings
                _stream.Write(customStringDatabase.Count);

                //Save Each Key and String
                foreach (KeyValuePair<string, string> customString in customStringDatabase)
                {
                    _stream.Write(customString.Key);
                    _stream.Write(customString.Value);
                }            
            }

            public override void DoLoad(int _version, BinaryReader _stream)
            {
                //Reset Database
                customStringDatabase = new Dictionary<string, string>();

                //Get Count
                int databaseCount = _stream.ReadInt32();

                for(int i = 0; i < databaseCount; i++)
                {
                    string key = _stream.ReadString();
                    string value = _stream.ReadString();

                    customStringDatabase.Add(key, value);
                }              
            }
        }
    }
}
