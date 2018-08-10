using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// V1.0 - SaveDataManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Manages the Saving and Loading of Game Data. Supports the saving of Custom Strings 
/// TODO: Make String Saving it's own Component Manager
/// </summary>
namespace RoboTools
{
    namespace Saving
    {
        public class SaveDataManager : MonoBehaviour
        {
            //V1 - Inital Setup
            public const int saveVersion = 1, 
                savesValidFrom = 1;

            public string saveLocation { get; private set; }

            public bool resetOnStart;

            private void Start()
            {
                if (resetOnStart)
                {
                    ResetSave();
                }
            }

            //------------------------------------Changeable Methods-------------------------------------
            //Reset the save to a blank state
            public void ResetSave()
            {
                Debug.Log("Resetting Save!");
            }

            //Saving for this specific game (Performed Last)
            private void GameSpecificSave(BinaryWriter bw)
            {
                ISavingManager[] managers = FindObjectsOfType<ISavingManager>();
                bw.Write(managers.Length);

                foreach (ISavingManager savingManager in managers)
                {
                    //Save UniqueID so we can identify this manager when loading
                    bw.Write(savingManager.uniqueID);
                    savingManager.DoSave(bw);
                }
            }

            //Loading for this specific game (Performed Last)
            private void GameSpecificLoad(int _version, BinaryReader br)
            {
                ISavingManager[] managers = FindObjectsOfType<ISavingManager>();

                int managersSize = br.ReadInt32();
                for (int i = 0; i < managersSize; i++)
                {
                    //Load Unique ID
                    char[] uniqueID = br.ReadChars(4);

                    foreach (ISavingManager savingManager in managers)
                    {
                        bool isTheSame = true;
                        for (int j = 0; j < 4; j++)
                        {
                            if (savingManager.uniqueID[j] != uniqueID[j])
                            {
                                isTheSame = false;
                                break;
                            }
                        }

                        if (isTheSame)
                        {
                            savingManager.DoLoad(_version, br);
                        }
                    }
                }
            }

            //Used for converting a save to the current version
            private void UpdateSave(string _location)
            {
                //Save the Game Data
                SaveGame(_location);
            }

            //----------------------------------Changeable Getters/Setters-----------------------------------

            //-----------------------------------------Useful Methods----------------------------------------

            public static void SaveVector3(BinaryWriter _stream, Vector3 _vector)
            {
                _stream.Write(_vector.x);
                _stream.Write(_vector.y);
                _stream.Write(_vector.z);
            }

            public static Vector3 LoadVector3(BinaryReader _stream)
            {
                return new Vector3(_stream.ReadSingle(), _stream.ReadSingle(), _stream.ReadSingle());
            }

            //------------------------------------Non-Changeable Methods-------------------------------------
            public void Save(string _location)
            {
                //Save the Game Data
                SaveGame(_location);
            }

            private void SaveGame(string _location)
            {
                saveLocation = _location;
                BinaryWriter bw = new BinaryWriter(File.Create(saveLocation));

                //Load Save Data
                bw.Write(saveVersion);

                //Load Game Specific Managers
                GameSpecificSave(bw);

                bw.Close();

                Debug.Log("Game Saved!");
            }

            public void Load(string _location)
            {
                saveLocation = _location;               
                bool saveLoaded = false;
                BinaryReader br = null;
                int version = -1;

                try
                {
                    if (File.Exists(saveLocation))
                    {
                        br = new BinaryReader(File.Open(saveLocation, FileMode.Open));
                        
                        //Load Save Data
                        version = br.ReadInt32();    

                        //Load Game Specific Managers
                        GameSpecificLoad(version, br);

                        if (version != saveVersion)
                        {
                            UpdateSave(_location);
                        }

                        saveLoaded = true;
                    }
                }
                catch (System.Exception err)
                {
                    Debug.Log("ERROR LOADING SAVE! " + err.Message);
                }
                finally
                {
                    br.Close();
                }

                if (!saveLoaded)
                {
                    ResetSave();
                }
                else
                {
                    Debug.Log("Save Loaded!");

                    Debug.Log("Version:" + version);       
                }
            }       

            public List<SaveData> PreviewSaves()
            {
                string folderName = Application.persistentDataPath + "/SaveSlots/";
                List<SaveData> validSaves = new List<SaveData>();

                //Create Folder if it dosen't exist
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                string[] allSaveFiles = Directory.GetFiles(folderName);
                foreach (string saveFile in allSaveFiles)
                {
                    try
                    {
                        string[] extensionCheck = saveFile.Split('.');
                        string[] slashCheck = extensionCheck[0].Split('/');

                        if (slashCheck[slashCheck.Length - 1].Substring(0, 8) == "SaveSlot" && extensionCheck[extensionCheck.Length - 1] == "save")
                        {
                            int slotID = int.Parse(slashCheck[slashCheck.Length - 1].Substring(8, 1));

                            if (slotID <= 0 || slotID > 4)
                            {
                                throw new Exception("Save Slot ID not valid");
                            }

                            slotID--;

                            //Load The SaveData
                            if (File.Exists(saveFile))
                            {
                                BinaryReader br = new BinaryReader(File.Open(saveFile, FileMode.Open));
                                int version = br.ReadInt32();

                                if(version >= savesValidFrom)
                                {
                                    validSaves.Add(new SaveData(saveFile, version));
                                }

                                br.Close();
                            }

                        }
                    }
                    catch (Exception err)
                    {
                        Debug.Log("Dodgy Save at " + saveFile + " :" + err.Message);
                    }
                }

                return validSaves;
            }
        } // SaveDataManager

        //Holds Data that is previewed
        public class SaveData
        {
            public string fileName { get; private set; }
            public int version { get; private set; }

            public SaveData(string _filename, int _version)
            {
                fileName = _filename;
                version = _version;
            }
        }
    }
}
