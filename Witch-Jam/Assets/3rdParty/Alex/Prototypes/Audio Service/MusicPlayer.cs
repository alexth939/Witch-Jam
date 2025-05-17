using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Runtime.Services
{
    public class MusicPlayer : IMusicPlayer
    {
        private readonly AudioSource _audioSource;
        private int _clipPointer = 0;
        private Action _pendingFadedOutCommand = null;
        private Action _pendingFadedInCommand = null;
        private List<AudioClip> _playlist;
        private MusicPlayerState _stateBackingField = MusicPlayerState.Idle;
        private float _stateInterpolationTime = 0f;
        private float _stateLifeTime = 0f;//seconds.
        private MusicSettings _musicSettings;

        public MusicPlayer(AudioSource source, MusicSettings musicSettings)
        {
            _audioSource = source;
            _musicSettings = musicSettings;
        }

        public float FadeOutDuration => _musicSettings.FadeOutDuration;

        public float FadeInDuration => _musicSettings.FadeInDuration;

        public float HaltDuration => _musicSettings.HaltDuration;

        public bool IsLooped { get; set; } = true;

        public bool IsPlaying => State is MusicPlayerState.Playing;

        private MusicPlayerState State
        {
            get => _stateBackingField;
            set
            {
                Debug.Log($"MusicPlayer [{GetHashCode()}]: was [{_stateBackingField}], now [{value}]");
                _stateBackingField = value;
            }
        }

        public void Pause() => PauseSmoothly(1f);

        public void PauseImmediately() => _audioSource.Pause();

        public void PauseSmoothly(float fadeOutDuration)
        {
            // Fade out volume then pause
        }

        public void Play(AudioClip clip)
        {
            List<AudioClip> playlist = new() { clip };
            Play(playlist);
        }

        public void Play(List<AudioClip> playlist)
        {
            if(State is MusicPlayerState.Idle)
            {
                _clipPointer = 0;
                _playlist = playlist.ToList();
                _audioSource.clip = _playlist[_clipPointer];

                _stateInterpolationTime = 0f;
                _stateLifeTime = 0f;
                State = MusicPlayerState.FadeIn;

                _audioSource.Play();
            }
            else if(State is MusicPlayerState.Playing)
            {
                _stateInterpolationTime = 0;
                _stateLifeTime = 0f;
                State = MusicPlayerState.FadeOut;

                _pendingFadedOutCommand = () =>
                {
                    _clipPointer = 0;
                    _playlist = playlist.ToList();
                    _audioSource.clip = _playlist[_clipPointer];

                    _stateInterpolationTime = 0f;
                    _stateLifeTime = 0f;
                    State = MusicPlayerState.FadeIn;

                    _audioSource.Play();
                };
            }
            else if(State is MusicPlayerState.FadeIn)
            {
            }
            else if(State is MusicPlayerState.FadeOut)
            {
            }
            else if(State is MusicPlayerState.Halt)
            {
            }
            else
            {
            }
        }

        public void PlayImmediately(AudioClip clip)
        {
            List<AudioClip> playlist = new() { clip };
            PlayImmediately(playlist);
        }

        public void PlayImmediately(List<AudioClip> clips)
        {
            State = MusicPlayerState.Playing;
            _stateLifeTime = 1f;
            _stateInterpolationTime = 1f;
            _audioSource.volume = 1f;
            _audioSource.Stop();
            _playlist = clips;
            _clipPointer = 0;
            _audioSource.clip = _playlist[_clipPointer];
            _audioSource.Play();
        }

        public void PlaySmoothly(AudioClip clip, float fadeInDuration)
        {
            _audioSource.clip = clip;
            _audioSource.volume = 0f;
            _audioSource.Play();
        }

        public void Stop()
        {
            if(State is MusicPlayerState.Playing)
            {
                _stateLifeTime = 0f;
                _stateInterpolationTime = 0f;
                State = MusicPlayerState.FadeOut;

                _pendingFadedOutCommand = () =>
                {
                    _audioSource.Stop();
                    _stateLifeTime = 0f;
                    _stateInterpolationTime = 1f;
                    State = MusicPlayerState.Idle;
                };
            }
            else if(State is MusicPlayerState.FadeOut)
            {
                _pendingFadedOutCommand = () =>
                {
                    _audioSource.Stop();
                    _stateLifeTime = 0f;
                    _stateInterpolationTime = 1f;
                    State = MusicPlayerState.Idle;
                };
            }
            else if(State is MusicPlayerState.FadeIn)
            {
                _pendingFadedInCommand = () =>
                {
                    _stateLifeTime = 0f;
                    _stateInterpolationTime = 0f;
                    State = MusicPlayerState.FadeOut;

                    _pendingFadedOutCommand = () =>
                    {
                        _audioSource.Stop();
                        _stateLifeTime = 0f;
                        _stateInterpolationTime = 1f;
                        State = MusicPlayerState.Idle;
                    };
                };
            }
        }

        public void StopImmediately()
        {
            State = MusicPlayerState.Idle;
            _stateLifeTime = 0f;
            _stateInterpolationTime = 1f;
            _audioSource.volume = 0f;
            _audioSource.Stop();
        }

        public void StopSmoothly(float fadeOutDuration)
        {
            // Fade out
        }

        public void Unpause() => UnpauseSmoothly(1f);

        public void UnpauseImmediately() => _audioSource.UnPause();

        public void UnpauseSmoothly(float fadeInDuration)
        {
            // Fade in volume after unpause
        }

        public void UpdateState(float timeDelta)
        {
            Action<float> stateUpdater = State switch
            {
                MusicPlayerState.Playing => UpdatePlayingState,
                MusicPlayerState.FadeIn => UpdateFadeInState,
                MusicPlayerState.FadeOut => UpdateFadeOutState,
                MusicPlayerState.Idle or _ => null,
            };

            stateUpdater?.Invoke(timeDelta);
        }

        private void UpdateFadeInState(float timeDelta)
        {
            _stateLifeTime += timeDelta;
            _stateInterpolationTime = Mathf.InverseLerp(0f, FadeInDuration, _stateLifeTime);
            _audioSource.volume = _stateInterpolationTime;

            bool shouldExitState = Mathf.Approximately(FadeInDuration, 0f);
            shouldExitState |= _stateInterpolationTime >= 1f;

            if(shouldExitState)
            {
                if(_pendingFadedInCommand is not null)
                {
                    Debug.Log($"invoking pending faded in command");
                    _pendingFadedInCommand.Invoke();
                    _pendingFadedInCommand = null;
                }
                else
                {
                    State = MusicPlayerState.Playing;
                    _stateLifeTime = 1f;
                    _audioSource.volume = 1f;
                    UpdateState(timeDelta);
                }
            }
        }

        private void UpdateFadeOutState(float timeDelta)
        {
            //Debug.Log($"state:FadeOut");

            _stateLifeTime += timeDelta;
            _stateInterpolationTime = Mathf.InverseLerp(0f, FadeOutDuration, _stateLifeTime);
            _audioSource.volume = 1f - _stateInterpolationTime;

            bool shouldExitState = Mathf.Approximately(FadeOutDuration, 0f);
            shouldExitState |= _stateInterpolationTime >= 1f;

            if(shouldExitState)
            {
                if(_pendingFadedOutCommand is not null)
                {
                    Debug.Log($"invoking pending faded out command");
                    _pendingFadedOutCommand.Invoke();
                    _pendingFadedOutCommand = null;
                }
                else
                {
                    if(_clipPointer < _playlist.Count - 1)
                    {
                        _clipPointer += 1;
                        _audioSource.clip = _playlist[_clipPointer];
                        _stateLifeTime = 0f;
                        _stateInterpolationTime = 0f;
                        State = MusicPlayerState.FadeIn;
                        _audioSource.Play();
                        UpdateState(timeDelta);
                    }
                    else if(IsLooped)
                    {
                        _clipPointer = 0;
                        _audioSource.clip = _playlist[_clipPointer];
                        _stateLifeTime = 0f;
                        _stateInterpolationTime = 0f;
                        State = MusicPlayerState.FadeIn;
                        _audioSource.Play();
                        UpdateState(timeDelta);
                    }
                    else
                    {
                        State = MusicPlayerState.Idle;
                    }
                }
            }
        }

        private void UpdatePlayingState(float timeDelta)
        {
            //Debug.Log($"state:Playing");

            _stateLifeTime += timeDelta;
            _stateInterpolationTime = Mathf.InverseLerp(
                0f,
                _audioSource.clip.length, _stateLifeTime);

            bool shouldExitState = _audioSource.time >= _audioSource.clip.length - FadeOutDuration;
            shouldExitState |= _stateInterpolationTime >= 1f;

            if(shouldExitState)
            {
                State = MusicPlayerState.FadeOut;
                _stateLifeTime = 0f;
                _stateInterpolationTime = 0f;
                UpdateState(timeDelta);
            }
        }
    }
}
