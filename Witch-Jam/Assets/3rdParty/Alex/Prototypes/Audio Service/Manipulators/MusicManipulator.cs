using System.Collections.Generic;
using Prototypes.GameServices;
using Runtime.Services;
using UnityEngine;

namespace Runtime.Manipulators
{
    [DisallowMultipleComponent]
    public class MusicManipulator : MonoBehaviour, IMusicManipulator
    {
        private const int DefaultPlayerIndex = 0;

        public bool IsMusicLoops { get; set; } = true;

        private IAudioService AudioService => ServiceContainer.Instance.Get<IAudioService>();

        public void PauseMusic() => PauseMusic(DefaultPlayerIndex);

        public void PauseMusic(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.Pause();
        }

        public void PauseMusicImmediately() => PauseMusicImmediately(DefaultPlayerIndex);

        public void PauseMusicImmediately(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.PauseImmediately();
        }

        public void PauseMusicSmoothly(float fadeOutDuration)
        {
            PauseMusicSmoothly(DefaultPlayerIndex, fadeOutDuration);
        }

        public void PauseMusicSmoothly(int playerIndex, float fadeOutDuration)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.PauseSmoothly(fadeOutDuration);
        }

        public void PlayMusic(AudioClip clip)
        {
            PlayMusic(DefaultPlayerIndex, clip);
        }

        public void PlayMusic(int playerIndex, AudioClip clip)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.Play(clip);
        }

        public void PlayMusic(List<AudioClip> clips)
        {
            PlayMusic(DefaultPlayerIndex, clips);
        }

        public void PlayMusic(int playerIndex, List<AudioClip> clips)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.Play(clips);
        }

        public void PlayMusicImmediately(AudioClip clip)
        {
            PlayMusicImmediately(DefaultPlayerIndex, clip);
        }

        public void PlayMusicImmediately(int playerIndex, AudioClip clip)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.PlayImmediately(clip);
        }

        public void PlayMusicSmoothly(AudioClip clip, float fadeInDuration)
        {
            PlayMusicSmoothly(DefaultPlayerIndex, clip, fadeInDuration);
        }

        public void PlayMusicSmoothly(int playerIndex, AudioClip clip, float fadeInDuration)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.PlaySmoothly(clip, fadeInDuration);
        }

        public void StopMusic() => StopMusic(DefaultPlayerIndex);

        public void StopMusic(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.Stop();
        }

        public void StopMusicImmediately() => StopMusicImmediately(DefaultPlayerIndex);

        public void StopMusicImmediately(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.StopImmediately();
        }

        public void StopMusicSmoothly(float fadeOutDuration)
        {
            StopMusicSmoothly(DefaultPlayerIndex, fadeOutDuration);
        }

        public void StopMusicSmoothly(int playerIndex, float fadeOutDuration)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.StopSmoothly(fadeOutDuration);
        }

        public void ToggleLoopMusicOff(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.IsLooped = false;
        }

        public void ToggleLoopMusicOn(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.IsLooped = true;
        }

        public void UnpauseMusic() => UnpauseMusic(DefaultPlayerIndex);

        public void UnpauseMusic(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.Unpause();
        }

        public void UnpauseMusicImmediately() => UnpauseMusicImmediately(DefaultPlayerIndex);

        public void UnpauseMusicImmediately(int playerIndex)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.UnpauseImmediately();
        }

        public void UnpauseMusicSmoothly(float fadeInDuration)
        {
            UnpauseMusicSmoothly(DefaultPlayerIndex, fadeInDuration);
        }

        public void UnpauseMusicSmoothly(int playerIndex, float fadeInDuration)
        {
            IMusicPlayer desiredPlayer = GetPlayer(playerIndex);
            desiredPlayer.UnpauseSmoothly(fadeInDuration);
        }

        private IMusicPlayer GetPlayer(int playerIndex) => AudioService.GetMusicPlayer(playerIndex);
    }
}
