using UnityEngine;
using System.Collections;

/// <summary>
/// V1.0 - SoundManager
/// Created By Robert Chisholm
/// Robo's Unity Tools 2018©
/// 
/// brief - Handles Sound Playing
/// </summary>
namespace RoboTools
{
    namespace Audio
    {
        public class SoundManager : MonoBehaviour
        {
            public static float masterVolume = 1f, musicVolume = 1f, sfxVolume = 1f, dVolume = 1f;
#if UNITY_EDITOR_WIN
            public float MasterVolume = 1f, MusicVolume = 1f, SfxVolume = 1f, DVolume = 1f;
#endif

            private float lastMav = -1f, lastMuv = -1f, lastsfV = -1f, lastDV = -1f;

            const float fadeTime = 0.5f;

            private AudioSource mSource, sfxSource, dSource;
            private AudioSourceInfo mSourceInfo, sfxSourceInfo, dSourceInfo;

            private bool mbeingUsed = false;

            // Use this for initialization
            void Awake()
            {
                masterVolume = Mathf.Clamp(PlayerPrefs.GetFloat("MAV", 1), 0, 1);
                musicVolume = Mathf.Clamp(PlayerPrefs.GetFloat("MV", 0.5f), 0, 1);
                sfxVolume = Mathf.Clamp(PlayerPrefs.GetFloat("SFXV", 1), 0, 1);
                dVolume = Mathf.Clamp(PlayerPrefs.GetFloat("DV", 1), 0, 1);

                lastMav = masterVolume;
                lastMuv = musicVolume;
                lastsfV = sfxVolume;
                lastDV = dVolume;

                //Create Children
                GameObject musicChild = new GameObject("Music");
                musicChild.transform.parent = transform;

                GameObject sfxChild = new GameObject("SFX");
                sfxChild.transform.parent = transform;

                GameObject dChild = new GameObject("Dialogue");
                dChild.transform.parent = transform;

                mSource = musicChild.AddComponent<AudioSource>();
                mSourceInfo = musicChild.AddComponent<AudioSourceInfo>();
                mSourceInfo.idealVolume = musicVolume;
                mSourceInfo.audioType = AudioSourceInfo.AudioType.Music;

                sfxSource = sfxChild.AddComponent<AudioSource>();
                sfxSourceInfo = sfxChild.AddComponent<AudioSourceInfo>();
                sfxSourceInfo.idealVolume = sfxVolume;
                sfxSourceInfo.audioType = AudioSourceInfo.AudioType.SFX;

                dSource = dChild.AddComponent<AudioSource>();
                dSourceInfo = dChild.AddComponent<AudioSourceInfo>();
                dSourceInfo.idealVolume = dVolume;
                dSourceInfo.audioType = AudioSourceInfo.AudioType.Dialogue;

                mSource.loop = true;

                DontDestroyOnLoad(this.gameObject);

#if UNITY_EDITOR_WIN
                MasterVolume = masterVolume;
                MusicVolume = musicVolume;
                SfxVolume = sfxVolume;
                DVolume = dVolume;
#endif
            }

            // Update is called once per frame
            void Update()
            {
                if (masterVolume != lastMav)
                {
                    PlayerPrefs.SetFloat("MAV", masterVolume);
                    lastMav = masterVolume;
                }

                if (musicVolume != lastMuv)
                {
                    PlayerPrefs.SetFloat("MV", musicVolume);
                    lastMuv = musicVolume;
                }

                if (sfxVolume != lastsfV)
                {
                    PlayerPrefs.SetFloat("SFXV", sfxVolume);
                    lastsfV = sfxVolume;
                }

                if (dVolume != lastDV)
                {
                    PlayerPrefs.SetFloat("DV", dVolume);
                    lastDV = dVolume;
                }

#if UNITY_EDITOR_WIN
                if (Input.GetKeyDown(KeyCode.G))
                {
                    if (mSource.isPlaying)
                        mSource.Stop();
                    else
                        mSource.Play();
                }

                masterVolume = MasterVolume;
                musicVolume = MusicVolume;
                sfxVolume = SfxVolume;
                dVolume = DVolume;
#endif
            }


            public void PlaySFX(AudioClip nMusic, float volumeScale)
            {
                if (sfxSource != null)
                {
                    sfxSource.PlayOneShot(nMusic, volumeScale);
                }
            }

            public void PlaySFX(AudioClip nMusic) { PlaySFX(nMusic, 1f); }

            public void PlayDialogue(AudioClip nMusic, float volumeScale)
            {
                if (dSource != null)
                {
                    StopDialogue();

                    dSource.clip = nMusic;
                    dSource.Play();
                }
            }
            public void PlayDialogue(AudioClip nMusic) { PlayDialogue(nMusic, 1f); }
            public void StopDialogue()
            {
                if (dSource != null)
                {
                    if (dSource.isPlaying)
                    {
                        dSource.Stop();
                    }
                }
            }


            public void PlayMusic(AudioClip nMusic)
            {
                StartCoroutine(ActualPlayMusic(nMusic));
            }

            private IEnumerator ActualPlayMusic(AudioClip nMusic)
            {
                if (mSource != null)
                {
                    //Wait for current track swap to finish
                    while (mbeingUsed)
                        yield return null;

                    mbeingUsed = true;

                    if (mSource.isPlaying)
                        yield return TransitionVolume(0f);

                    mSource.Stop();
                    mSource.clip = nMusic;
                    mSource.Play();

                    yield return TransitionVolume(1f);

                    mbeingUsed = false;
                }
            }

            public void StopMusic()
            {
                StartCoroutine(ActualStopMusic());
            }

            private IEnumerator ActualStopMusic()
            {
                //Wait for current track swap to finish
                while (mbeingUsed)
                    yield return null;

                mbeingUsed = true;

                if (mSource.isPlaying)
                    yield return TransitionVolume(0f);

                mSource.Stop();
                mbeingUsed = false;
            }

            private IEnumerator TransitionVolume(float endVolume)
            {
                float startTime = Time.realtimeSinceStartup;
                float startVolume = mSource.volume;

                while ((Time.realtimeSinceStartup - startTime) < fadeTime)
                {
                    mSourceInfo.idealVolume = Mathf.Lerp(startVolume, endVolume, (Time.realtimeSinceStartup - startTime) / fadeTime);
                    yield return null;
                }

                mSourceInfo.idealVolume = endVolume;
            }

            public void SetMusicPitch(float value)
            {
                mSource.pitch = value;
            }
        }
    }
}
