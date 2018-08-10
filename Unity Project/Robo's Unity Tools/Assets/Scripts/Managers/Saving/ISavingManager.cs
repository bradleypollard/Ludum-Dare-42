using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// V1.0 - ISavingManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Abstract Class used to allow the saving of additional data
/// </summary>
namespace RoboTools
{
    namespace Saving
    {
        public abstract class ISavingManager : MonoBehaviour
        {
            abstract public void DoSave(BinaryWriter _stream);
            abstract public void DoLoad(int _version, BinaryReader _stream);
            virtual public char[] uniqueID
            {
                get { return new char[4] { 'I', 'S', 'M', '_' }; }
            }
        }
    }
}
