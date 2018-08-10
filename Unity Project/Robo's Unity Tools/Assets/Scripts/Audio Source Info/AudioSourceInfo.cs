using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// V1.0 - AudioSourceInfo
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Used in conjunction with Sound Manager to allow Player to change Volume
/// </summary>
namespace RoboTools
{
    namespace Audio
    {
        [RequireComponent(typeof(AudioSource))]
        public class AudioSourceInfo : MonoBehaviour
        {

            public enum AudioType { Music, SFX, Dialogue }

            public AudioType audioType;
            public float idealVolume = 1f;

            private AudioSource audioSource;

            void Start()
            {
                audioSource = GetComponent<AudioSource>();
            }

            // Update is called once per frame
            void Update()
            {
                float modifier = 1f;

                if (audioType == AudioType.Music)
                    modifier = SoundManager.musicVolume * SoundManager.masterVolume;
                else if (audioType == AudioType.SFX)
                    modifier = SoundManager.sfxVolume * SoundManager.masterVolume;
                else if (audioType == AudioType.Dialogue)
                    modifier = SoundManager.dVolume * SoundManager.masterVolume;

                audioSource.volume = idealVolume * modifier;
            }
        }
    }
}
