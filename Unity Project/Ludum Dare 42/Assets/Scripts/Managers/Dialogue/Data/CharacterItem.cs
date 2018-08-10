using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - CharacterItem
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Stores an instance of a Loaded Character
/// </summary>
namespace RoboTools
{
    namespace Dialogue
    {
        public class CharacterItem
        {
            public string ID { get; private set; } //How we will identify this character
            public string printName, //How the character will be shown ingame
                prefabResource, gibberResource; //File's to be loaded

            public GameObject prefab, instantiatedPrefab;
            public AudioClip gibberNoise;

            private bool isTalking;
            public Vector3 localScale;

            public CharacterItem(string _id)
            {
                ID = _id;
                printName = "";
                prefabResource = "";
                gibberResource = "";
                prefab = null;
                instantiatedPrefab = null;
                gibberNoise = null;
                isTalking = false;
                localScale = Vector3.one;
            }

            public void SetIsTalking(bool _isTalking) { isTalking = _isTalking; }
            public bool GetIsTalking() { return isTalking; }
        }
    }
}