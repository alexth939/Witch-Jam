using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Runtime.Services
{
    [DisallowMultipleComponent]
    public class AudioServiceBehaviour : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private MusicSettings _musicSettings;

        private Dictionary<int, IMusicPlayer> _musicPlayers = new();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public IMusicPlayer GetMusicPlayer(int playerIndex)
        {
            if(_musicPlayers.TryGetValue(playerIndex, out IMusicPlayer player))
                return player;

            IMusicPlayer musicPlayer = CreateMusicPlayer(playerIndex);

            return musicPlayer;
        }

        private IMusicPlayer CreateMusicPlayer(int playerIndex)
        {
            AudioSource musicAudioSource = gameObject.AddComponent<AudioSource>();
            musicAudioSource.loop = false;
            musicAudioSource.playOnAwake = false;
            MusicPlayer musicPlayer = new(musicAudioSource, _musicSettings);
            _musicPlayers.Add(playerIndex, musicPlayer);

            if(_musicGroup != null)
                musicAudioSource.outputAudioMixerGroup = _musicGroup;
            else
                Debug.LogWarning("Music group not assigned in AudioService!");

            return musicPlayer;
        }

        public void ForceSetMusicGroupVolume(float volume)
        {
            string musicVolumeParamName = "MusicVolume";

            volume = Mathf.Clamp01(volume);

            float volumeDb = Mathf.Approximately(volume, 0f) ? -80f : Mathf.Log10(volume) * 20f;

            if(_musicGroup == null)
            {
                Debug.LogWarning($"{nameof(_musicGroup)} is not assigned!");

                return;
            }

            if(_musicGroup.audioMixer == null)
            {
                Debug.LogWarning("AudioMixer is missing in assigned MusicGroup.");

                return;
            }

            bool isValueApplied = _musicGroup.audioMixer.SetFloat(musicVolumeParamName, volumeDb);

            if(isValueApplied == false)
            {
                Debug.LogWarning($"Failed to set '{musicVolumeParamName}' on AudioMixer. " +
                    $"Make sure it's exposed.");
            }
        }

        public void ForceSetMasterGroupVolume(float volume)
        {
            string musicVolumeParamName = "MasterVolume";

            volume = Mathf.Clamp01(volume);

            float volumeDb = Mathf.Approximately(volume, 0f) ? -80f : Mathf.Log10(volume) * 20f;

            if(_musicGroup == null)
            {
                Debug.LogWarning($"{nameof(_musicGroup)} is not assigned!");

                return;
            }

            if(_musicGroup.audioMixer == null)
            {
                Debug.LogWarning("AudioMixer is missing in assigned MasterGroup.");

                return;
            }

            bool isValueApplied = _musicGroup.audioMixer.SetFloat(musicVolumeParamName, volumeDb);

            if(isValueApplied == false)
            {
                Debug.LogWarning($"Failed to set '{musicVolumeParamName}' on AudioMixer. " +
                    $"Make sure it's exposed.");
            }
        }

        public void ForceSetSFXGroupVolume(float volume)
        {
            string musicVolumeParamName = "SFXVolume";

            volume = Mathf.Clamp01(volume);

            float volumeDb = Mathf.Approximately(volume, 0f) ? -80f : Mathf.Log10(volume) * 20f;

            if(_musicGroup == null)
            {
                Debug.LogWarning($"{nameof(_musicGroup)} is not assigned!");

                return;
            }

            if(_musicGroup.audioMixer == null)
            {
                Debug.LogWarning("AudioMixer is missing in assigned SFXGroup.");

                return;
            }

            bool isValueApplied = _musicGroup.audioMixer.SetFloat(musicVolumeParamName, volumeDb);

            if(isValueApplied == false)
            {
                Debug.LogWarning($"Failed to set '{musicVolumeParamName}' on AudioMixer. " +
                    $"Make sure it's exposed.");
            }
        }

        private void FixedUpdate()
        {
            foreach(IMusicPlayer player in _musicPlayers.Values)
            {
                player.UpdateState(Time.deltaTime);
            }
        }
    }

    [System.Serializable]
    public class MusicSettings
    {
        /// <summary>
        ///     Duration (in seconds) for fading music out (volume from 1 to 0).
        /// </summary>
        [field: SerializeField] public float FadeOutDuration { get; set; }

        /// <summary>
        ///     Duration (in seconds) for fading music in (volume from 0 to 1).
        /// </summary>
        [field: SerializeField] public float FadeInDuration { get; set; }

        /// <summary>
        ///     Silence (in seconds) between the end of the current clip (after fade out)
        ///     and the start of the next clip (before fade in), during sequential playback.
        ///     Ignored in 'Immediately', 'Smoothly', or 'Blend' methods.
        /// </summary>
        [field: SerializeField] public float HaltDuration { get; set; }
    }
}
