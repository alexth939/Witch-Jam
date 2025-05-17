namespace Runtime.Services
{
    public interface IAudioService
    {
        IMusicPlayer GetMusicPlayer(int playerIndex);

        /// <summary>
        ///     Ignores game configuration settings and sets music group volume.
        ///     <br/>
        ///     Normalized <paramref name="volume"/> value is automatically converted to decibels logarythmically:
        ///     <code>
        ///     0f -> (-80db)
        ///     1f -> (0db)
        ///     </code>
        /// </summary>
        /// <param name="volume">Clamped to range 0f to 1f</param>
        void ForceSetMusicGroupVolume(float volume);

        /// <summary>
        ///     Ignores game configuration settings and sets master group volume.
        ///     <br/>
        ///     Normalized <paramref name="volume"/> value is automatically converted to decibels logarythmically:
        ///     <code>
        ///     0f -> (-80db)
        ///     1f -> (0db)
        ///     </code>
        /// </summary>
        /// <param name="volume">Clamped to range 0f to 1f</param>
        void ForceSetMasterGroupVolume(float volume);

        /// <summary>
        ///     Ignores game configuration settings and sets sfx group volume.
        ///     <br/>
        ///     Normalized <paramref name="volume"/> value is automatically converted to decibels logarythmically:
        ///     <code>
        ///     0f -> (-80db)
        ///     1f -> (0db)
        ///     </code>
        /// </summary>
        /// <param name="volume">Clamped to range 0f to 1f</param>
        void ForceSetSFXGroupVolume(float volume);
    }
}
