using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Services
{
    public interface IMusicPlayer
    {
        bool IsPlaying { get; }

        bool IsLooped { get; set; }

        void Pause();

        void PauseImmediately();

        void PauseSmoothly(float fadeOutDuration);

        void Play(AudioClip clip);

        void Play(List<AudioClip> playlist);

        void PlayImmediately(AudioClip clip);

        void PlaySmoothly(AudioClip clip, float fadeInDuration);

        void Stop();

        void StopImmediately();

        void StopSmoothly(float fadeOutDuration);

        void UpdateState(float timeDelta);

        void Unpause();

        void UnpauseImmediately();

        void UnpauseSmoothly(float fadeInDuration);
    }
}
