using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Manipulators
{
    /// <summary>
    ///     Provides simple access for designers to control the audio system via Inspector or scripting.
    ///     <br/>
    ///     Internally delegates to music and SFX players (each supporting multiple players).
    ///     <br/>
    ///     Defaults to player 0 unless a specific player is provided.
    /// </summary>
    public interface IMusicManipulator
    {
        /// <summary>
        ///     Default looping behavior for all music players.
        ///     <br/>
        ///     Also affects currently running music.
        /// </summary>
        bool IsMusicLoops { get; set; }

        // ------- Play (predefined configuration [Halt, FadeIn, FadeOut]) -------

        /// <summary>
        ///     Stops current music using <see cref="FadeOutDuration"/>, clears the playlist,
        ///     and starts playing the given clip with <see cref="FadeInDuration"/> on player 0.
        /// </summary>
        void PlayMusic(AudioClip clip);

        /// <summary>
        ///     Same as <see cref="PlayMusic(AudioClip)"/>, but on the specified player.
        /// </summary>
        void PlayMusic(int playerIndex, AudioClip clip);

        /// <summary>
        ///     Stops current music using <see cref="FadeOutDuration"/>, clears the playlist,
        ///     and starts playing the first clip from the array using <see cref="FadeInDuration"/> on player 0.
        /// </summary>
        void PlayMusic(List<AudioClip> clips);

        /// <summary>
        ///     Same as <see cref="PlayMusic(AudioClip[])"/>, but on the specified player.
        /// </summary>
        void PlayMusic(int playerIndex, List<AudioClip> clips);

        // ------- Play (Overrides configuration) -------

        /// <summary>
        ///     Immediately stops music (no fade), clears the playlist,
        ///     and starts playing the given clip without delay or halt.
        ///     Uses player 0.
        /// </summary>
        void PlayMusicImmediately(AudioClip clip);

        /// <summary>
        ///     Same as <see cref="PlayMusicImmediately(AudioClip)"/>, but on the specified player.
        /// </summary>
        void PlayMusicImmediately(int playerIndex, AudioClip clip);

        /// <summary>
        ///     Stops current music using <see cref="FadeOutDuration"/>, clears the playlist,
        ///     and starts playing the given clip with the specified fade-in duration.
        ///     Uses player 0.
        /// </summary>
        void PlayMusicSmoothly(AudioClip clip, float fadeInDuration);

        /// <summary>
        ///     Same as <see cref="PlayMusicSmoothly(AudioClip, float)"/>, but on the specified player.
        /// </summary>
        void PlayMusicSmoothly(int playerIndex, AudioClip clip, float fadeInDuration);

        // ------- Stop (predefined configuration) -------

        /// <summary>
        ///     Stops music on player 0 using <see cref="FadeOutDuration"/>.
        /// </summary>
        void StopMusic();

        /// <summary>
        ///     Stops music on the specified player using <see cref="FadeOutDuration"/>.
        /// </summary>
        void StopMusic(int playerIndex);

        /// <summary>
        ///     Immediately stops music on player 0.
        /// </summary>
        void StopMusicImmediately();

        /// <summary>
        ///     Immediately stops music on the specified player.
        /// </summary>
        void StopMusicImmediately(int playerIndex);

        /// <summary>
        ///     Smoothly stops music on player 0 using the provided fade-out duration.
        /// </summary>
        void StopMusicSmoothly(float fadeOutDuration);

        /// <summary>
        ///     Smoothly stops music on the specified player using the provided fade-out duration.
        /// </summary>
        void StopMusicSmoothly(int playerIndex, float fadeOutDuration);

        // ------- Loop Control -------

        /// <summary>
        ///     Enables looping on the specified music player.
        /// </summary>
        void ToggleLoopMusicOn(int playerIndex);

        /// <summary>
        ///     Disables looping on the specified music player.
        /// </summary>
        void ToggleLoopMusicOff(int playerIndex);

        // ------- Pause -------

        /// <summary>
        ///     Pauses music on player 0 using <see cref="FadeOutDuration"/>.
        /// </summary>
        void PauseMusic();

        /// <summary>
        ///     Pauses music on the specified player using <see cref="FadeOutDuration"/>.
        /// </summary>
        void PauseMusic(int playerIndex);

        /// <summary>
        ///     Immediately pauses music on player 0 (no fade).
        /// </summary>
        void PauseMusicImmediately();

        /// <summary>
        ///     Immediately pauses music on the specified player (no fade).
        /// </summary>
        void PauseMusicImmediately(int playerIndex);

        /// <summary>
        ///     Smoothly pauses music on player 0 using the provided fade-out duration.
        /// </summary>
        void PauseMusicSmoothly(float fadeOutDuration);

        /// <summary>
        ///     Smoothly pauses music on the specified player using the provided fade-out duration.
        /// </summary>
        void PauseMusicSmoothly(int playerIndex, float fadeOutDuration);

        // ------- Unpause -------

        /// <summary>
        ///     Resumes playback on player 0 using <see cref="FadeInDuration"/>.
        /// </summary>
        void UnpauseMusic();

        /// <summary>
        ///     Resumes playback on the specified player using <see cref="FadeInDuration"/>.
        /// </summary>
        void UnpauseMusic(int playerIndex);

        /// <summary>
        ///     Immediately resumes playback on player 0.
        /// </summary>
        void UnpauseMusicImmediately();

        /// <summary>
        ///     Immediately resumes playback on the specified player.
        /// </summary>
        void UnpauseMusicImmediately(int playerIndex);

        /// <summary>
        ///     Smoothly resumes playback on player 0 using the provided fade-in duration.
        /// </summary>
        void UnpauseMusicSmoothly(float fadeInDuration);

        /// <summary>
        ///     Smoothly resumes playback on the specified player using the provided fade-in duration.
        /// </summary>
        void UnpauseMusicSmoothly(int playerIndex, float fadeInDuration);
    }
}