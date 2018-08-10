using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - DialogueInstance
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Stores an instance of a Loaded Dialogue
/// </summary>
namespace RoboTools
{
    namespace Dialogue
    {
        public class DialogueInstance
        {
            public Dictionary<string, CharacterItem> currentCharacters;
            public Dictionary<string, SpeechItem> currentDialogue;

            public int defaultDialogueCount;

            public DialogueInstance()
            {
                currentCharacters = new Dictionary<string, CharacterItem>();
                currentDialogue = new Dictionary<string, SpeechItem>();
                defaultDialogueCount = 0;
            }

        }
    }
}