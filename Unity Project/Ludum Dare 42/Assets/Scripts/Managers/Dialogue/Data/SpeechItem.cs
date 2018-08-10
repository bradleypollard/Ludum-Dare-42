using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// V1.0 - SpeechItem
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Holds a single piece of Dialogue
/// </summary>
namespace RoboTools
{
    namespace Dialogue
    {
        //What you expect. The dialogue that appears inside of a box
        public class SpeechItem
        {
            public SpeechItem(int _id)
            {
                id = _id;
            }

            public int id { get; private set; }

            public string characterID, audioPrefabName, musicClipName;
            public float customSpeed, customSize;

            public AudioClip audioPrefab, musicClip;

            public string animationName;
            public object[] animationParams;

            //Text Saving
            public bool skipText, requiresInput;
            public string finalFormattedText; //What will appear in the Text Box
            public string finalReadText; //How the text will be read, includes spaces for *(Effect)* 
            public List<Dictionary<Effect.EffectType, Effect>> textEffects; //For each character in finalReadText there is a array represing what effects are applied to it
        }

        public class Effect
        {
            public enum EffectType { Size, Colour, Font, Bold, Italic, ShakeStart, ShakeEnd, Pause }

            public EffectType effectType { get; private set; }
            public EffectData effectData { get; private set; }

            public Effect(EffectType _effectType, EffectData _effectData)
            {
                effectType = _effectType;
                effectData = _effectData;
            }
        }

        public abstract class EffectData { }

        /// <summary>
        /// Used for ShakeStart / ShakeEnd 
        /// When no extra data is required
        /// </summary>
        public class NullData : EffectData { }

        /// <summary>
        /// Used for Font 
        /// When a string is required
        /// </summary>
        public class StringData : EffectData
        {
            public string data { get; private set; }
            public StringData(string _data) { data = _data; }
        }

        /// <summary>
        /// Used for Size 
        /// When an int is required
        /// </summary>
        public class IntData : EffectData
        {
            public int data { get; private set; }
            public IntData(int _data) { data = _data; }
        }

        /// <summary>
        /// Used for  Pause 
        /// When an int is required
        /// </summary>
        public class FloatData : EffectData
        {
            public float data { get; private set; }
            public FloatData(float _data) { data = _data; }
        }

        /// <summary>
        /// Used for Colour
        /// When an colour is required
        /// </summary>
        public class ColourData : EffectData
        {
            public Color data { get; private set; }
            public ColourData(Color _data) { data = _data; }
        }
    }
}
