using UnityEngine;

namespace ChocoDark.GlobalAudio
{
    public enum PlayMode
    {
        Interrupt,
        Additive,
        Ignore
    }

    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudioSource : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [field: SerializeField] public Channel Channel { get; protected set; }
        [field: SerializeField] public PlayMode PlayMode { get; protected set; }

        [field: SerializeField] public bool IgnoreChannelVolume { get; set; } = false;

        private void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            GlobalAudio.onChannelPlayClip.AddListener(OnChannelPlayClip);
            GlobalAudio.onChannelVolumeChange.AddListener(OnChannelVolumeChange);
            GlobalAudio.onChannelPause.AddListener(OnChannelPause);

            // initialize audio source volume
            if (Channel != Channel.None)
                SetVolume(GlobalAudio.GetChannelVolume(Channel));
        }


        /// <summary>
        /// Plays an audio clip, only if the play mode is not set to 'Ignore'. </summary>
        /// <param name="volume">
        /// Sets the volume of the clip if the play mode is set to 'Additive'. </param>
        public void PlayClip(AudioClip clip, float volume = 1)
        {
            // cannot play
            if (audioSource == null || clip == null)
                return;

            // cannot play
            if (PlayMode == PlayMode.Ignore)
                return;

            if (PlayMode == PlayMode.Additive)
            {
                audioSource.PlayOneShot(clip, volume);
            }
            else if (PlayMode == PlayMode.Interrupt)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                //audioSource.volume = minVolume;
                //Singleton.musicAudioSource.PlayDelayed(5);
                audioSource.Play();
            }
        }

        /// <summary>
        /// Pauses or resumes the playback of the AudioSource. </summary>
        public void Pause(bool pause)
        {
            // no audio source
            if (audioSource == null)
                return;

            // bypass channel filter if is setted to ALL
            if (Channel != Channel.All)
            {
                // cannot play
                if (PlayMode == PlayMode.Ignore)
                    return;
            }

            if (pause)
                audioSource.Pause();
            else
                audioSource.UnPause();
        }

        /// <summary>
        /// Sets the volume of the AudioSource. </summary>
        /// <remarks>
        /// You can use the 'IgnoreChannelVolume' property if you want to maintain a fixed volume. </remarks>
        public void SetVolume(float volume)
        {
            // no audio source
            if (audioSource == null)
                return;

            audioSource.volume = volume;
        }


        #region Events
        void OnChannelPlayClip(Channel channel, AudioClip clip)
        {
            // No channel selected
            if (Channel == Channel.None || channel == Channel.None)
                return;

            // bypass channel filter if is setted to ALL
            if (channel != Channel.All && Channel != Channel.All)
            {
                // filter by channel
                if (channel != Channel)
                    return;
            }

            PlayClip(clip, GlobalAudio.SFXVolume);
        }
        void OnChannelVolumeChange(Channel channel, float volume)
        {
            if (IgnoreChannelVolume)
                return;

            // No channel selected
            if (Channel == Channel.None || channel == Channel.None)
                return;

            // bypass channel filter if is setted to ALL
            if (channel != Channel.All && Channel != Channel.All)
            {
                // filter by channel
                if (channel != Channel)
                    return;
            }

            SetVolume(volume);
        }
        void OnChannelPause(Channel channel, bool pause)
        {
            // No channel selected
            if (Channel == Channel.None || channel == Channel.None)
                return;

            // bypass channel filter if is setted to ALL
            if (channel != Channel.All && Channel != Channel.All)
            {
                // filter by channel
                if (channel != Channel)
                    return;
            }

            Pause(pause);
        }
        #endregion

    }
}