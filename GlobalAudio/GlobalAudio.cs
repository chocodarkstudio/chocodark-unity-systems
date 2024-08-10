using MusicClips;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

namespace ChocoDark.GlobalAudio
{

    public class GlobalAudio : MonoBehaviour
    {
        static GlobalAudio Singleton;

        [SerializeField] AudioMixer masterMixer;
        static readonly Dictionary<Channel, float> channelsVolume = new();

        // Effect Volume
        public static float SFXVolume
        {
            get => GetChannelVolume(Channel.SFX);
            set => SetChannelVolume(Channel.SFX, value);

        }

        // Music Volume
        public static float MusicVolume
        {
            get => GetChannelVolume(Channel.Music);
            set => SetChannelVolume(Channel.Music, value);
        }

        [Header("Clips Presets")]
        [SerializeField] UIClipsPreset uiClipsPreset;
        public static UIClipsPreset UIClips => Singleton.uiClipsPreset;


        [SerializeField] MusicClipsPreset musicClipsPreset;
        public static MusicClipsPreset MusicClips => Singleton.musicClipsPreset;


        [SerializeField] GeneralClipsPreset generalClipsPreset;
        public static GeneralClipsPreset GeneralClips => Singleton.generalClipsPreset;

        public static bool IsMusicLoopPaused { get; private set; }

        // Events
        public static readonly UnityEvent<Channel, float> onChannelVolumeChange = new();
        public static readonly UnityEvent<Channel, AudioClip> onChannelPlayClip = new();
        public static readonly UnityEvent<Channel, bool> onChannelPause = new();

        private void Awake()
        {
            if (SingletonExists())
            {
                Debug.LogWarning($"Another instance of {nameof(GlobalAudio)} already exists!! Make sure only one exists");
                return;
            }
            Singleton = this;
        }


        static bool SingletonExists(bool displayError = false)
        {
            if (Singleton == null)
            {
                if (displayError)
                    Debug.LogError($"No GlobalAudio instance was created!");
                return false;
            }
            return true;
        }


        #region Mixer
        public static void SetMixerFloat(string property, float value)
        {
            if (!SingletonExists(displayError: true))
                return;

            if (Singleton.masterMixer == null)
            {
                Debug.LogWarning($"MasterMixer has not been assigned!");
                return;
            }

            Singleton.masterMixer.SetFloat(property, value);
        }

        public static float GetMixerFloat(string property)
        {
            if (!SingletonExists(displayError: true))
                return 0;

            if (Singleton.masterMixer == null)
            {
                Debug.LogWarning($"MasterMixer has not been assigned!");
                return 0;
            }

            bool hasProperty = Singleton.masterMixer.GetFloat(property, out float value);
            if (!hasProperty)
                return 0;

            return value;
        }
        #endregion


        #region Play
        /// <summary>
        /// Plays an audio clip in the specified channel. </summary>
        public static void PlayInChannel(AudioClip clip, Channel channel)
        {
            if (clip == null)
                return;

            onChannelPlayClip.Invoke(channel, clip);
        }
        /// <summary> Plays a sound effect audio clip (SFX channel). </summary>
        public static void PlaySFX(AudioClip clip) => PlayInChannel(clip, Channel.SFX);
        /// <summary> Plays a music audio clip (Music channel). </summary>
        public static void PlayMusic(AudioClip clip) => PlayInChannel(clip, Channel.Music);

        /// <summary>
        /// Create a temp AudioSource, Play an audio clip and destroy it when finished </summary>
        public static void PlayAtPoint(AudioClip clip, Vector3? worldPos = null,
            float volume = 1, float pitchRandomizer = 0, float spatialBlend = 0.7f)
        {
            if (clip == null)
                return;

            // Create the audio source
            GameObject gameObject = new("One shot audio");
            gameObject.transform.position = worldPos.Value;
            AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));

            // configure
            audioSource.clip = clip;
            audioSource.spatialBlend = spatialBlend;
            audioSource.volume = Mathf.Min(volume, SFXVolume);
            audioSource.pitch = 1 + Random.Range(-pitchRandomizer, pitchRandomizer);

            // play and destroy
            audioSource.Play();
            Destroy(gameObject, 1 + clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }
        #endregion


        public static void PauseChannel(Channel channel, bool pause)
        {
            onChannelPause.Invoke(channel, pause);
        }


        #region Volume
        public static float GetChannelVolume(Channel channel)
        {
            if (channel == Channel.None)
                return 0;

            return channelsVolume.GetValueOrDefault(channel, 1f);
        }
        public static void SetChannelVolume(Channel channel, float volume)
        {
            channelsVolume[channel] = volume;
            onChannelVolumeChange.Invoke(channel, volume);
        }
        #endregion

        /*
        public static void PlayMusicLoop(LoopOption loopOption, bool randomize = false)
        {
            if (Singleton == null)
            {
                Debug.Log("GlobalAudio not found!");
                return;
            }

            // already in that state
            if (MusicClipsPreset.CurrentLoop == loopOption &&
                MusicClipsPreset.RandomizeIndex == randomize)
                return;

            MusicClipsPreset.CurrentLoop = loopOption;
            MusicClipsPreset.RandomizeIndex = randomize;
            MusicClips.RestartLoop();

            // play now if loop isnt paused
            if (!IsMusicLoopPaused)
                PlayMusic(MusicClips.Next());
        }
        */
        /*
        public static void StopChannelClip(AudioClip clip)
        {
            // invalid clip
            if (clip == null)
                return;

            // no playing clip
            if (Singleton.musicAudioSource.clip == null)
                return;

            // no matching clip
            if (Singleton.musicAudioSource.clip.name != clip.name)
                return;

            // stop it
            Singleton.musicAudioSource.Stop();
            Singleton.musicAudioSource.clip = null;
        }
        */
    }
}