using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Audio;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftApplesGames.RetroRacketRevolution.Audio
{
    public enum PlayBackMode { PlayOne, PlayAll, Shuffle }

    public class MusicManager : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public PlayBackMode playbackMode = PlayBackMode.PlayAll;
        [BoxGroup("General Settings")] public bool playOnStart;
        [BoxGroup("General Settings")] public bool repeat;
        [BoxGroup("General Settings")] public float fadeTimeInSeconds;
        [BoxGroup("General Settings")] public PlayList playList;

        [FoldoutGroup("Events")] public UnityEvent SongStartedEvent;
        [FoldoutGroup("Events")] public UnityEvent SongEndedEvent;
        [FoldoutGroup("Events")] public UnityEvent AllSongsEndedEvent;

        // Public properties

        // Private fields
        private int _numberOfMusicClips;
        private AudioSource _audioSource;
        private float _silentVolume;
        private float _playVolume;
        private int _currSongIndex;

        private bool _isPlaying;
        private bool _isPaused;

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _numberOfMusicClips = playList.NumberOfSongs;
            _audioSource = GetComponent<AudioSource>();
            _silentVolume = 0.0f;
            _playVolume = _audioSource.volume;
            _currSongIndex = -1;
            _isPlaying = false;
            _isPaused = false;
        }
    
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            if (_numberOfMusicClips == 0)
            {
                Debug.LogError("Music player has no music clips!");
                return;
            }

            if (playOnStart)
            {
                Play();
            }
        }

        /// <summary>
        /// Check for AudioSource status
        /// </summary>
        private void Update()
        {
            if (!_isPlaying)
            {
                return;
            }

            // Current song has finished
            if (!_audioSource.isPlaying)
            {
                switch (playbackMode)
                {
                    case PlayBackMode.PlayAll:
                        PlayNext();
                        break;
                    case PlayBackMode.Shuffle:
                        Shuffle();
                        break;
                    case PlayBackMode.PlayOne:
                        if (repeat)
                        {
                            PlaySong(_currSongIndex);
                        }
                        break;
                }
            }
        }
        #endregion

        #region PublicMethods

        [BoxGroup("Controls")]
        [Button("Play")]
        public void Play()
        {
            PlaySong(0);
        }

        /// <summary>
        /// Play the given song by index
        /// </summary>
        /// <param name="songIndex"></param>
        public void PlaySong(int songIndex)
        {
            // Stop current song, if playing
            _audioSource.Stop();
            FadeIn(songIndex);
            _currSongIndex = songIndex;
        }

        /// <summary>
        /// Play the next song in the list
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Next")]
        public void PlayNext()
        {
            if (_currSongIndex < _numberOfMusicClips - 1)
            {
                PlaySong(_currSongIndex+1);
            }
            else
            {
                // Go back to start
                if (repeat)
                {
                    PlaySong(0);
                }
            }
        }

        /// <summary>
        /// Play the previous song in the list
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Previous")]
        public void PlayPrevious()
        {
            if (_currSongIndex > 0)
            {
                PlaySong(_currSongIndex - 1);
            }
        }

        /// <summary>
        /// Play a random song in the list
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Shuffle")]
        public void Shuffle()
        {
            System.Random rand = new System.Random();
            int songIndex = rand.Next(0, _numberOfMusicClips - 1);
            PlaySong(songIndex);
        }
        #endregion

        /// <summary>
        /// Pause current playing song
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Pause")]
        public void Pause()
        {
            if (!_isPaused)
            {
                _audioSource.Pause();
                _isPaused = true;
            }
        }

        /// <summary>
        /// Resume playing the current song
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Resume")]
        public void Resume()
        {
            if (_isPaused)
            {
                _audioSource.Play();
                _isPaused = false;
            }
        }

        /// <summary>
        /// Stop playing the play list
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Stop")]
        public void Stop()
        {
            FadeOut(StopMusic);
        }

        /// <summary>
        /// Reset current playlist to start
        /// </summary>
        [BoxGroup("Controls")]
        [Button("Reset")]
        public void Reset()
        {
            FadeOut(StopMusic);
            _currSongIndex = 0;
        }

	    #region PrivateMethods

        /// <summary>
        /// Stops the audiosource and resets playlist
        /// </summary>
        private void StopMusic()
        {
            _audioSource.Stop();
        }

        /// <summary>
        /// Sync wrapper to FadeInAsync
        /// </summary>
        /// <param name="clipIndex"></param>
        private void FadeIn(int clipIndex)
        {
            StartCoroutine(FadeInAsync(clipIndex));
        }

        /// <summary>
        /// Sync wrapper to FadeOutAsync
        /// </summary>
        /// <param name="clipIndex"></param>
        private void FadeOut(Action callBack)
        {
            StartCoroutine(FadeOutAsync(callBack));
        }

        /// <summary>
        /// Fade in music source async
        /// </summary>
        /// <param name="clipIndex"></param>
        /// <returns></returns>
        private IEnumerator FadeInAsync(int clipIndex)
        {
            float time = 0;
            _audioSource.volume = _silentVolume;
            _audioSource.clip = playList.GetSongAtIndex(clipIndex);
            _audioSource.Play();
            while (time < fadeTimeInSeconds)
            {
                _audioSource.volume = Mathf.Lerp(_silentVolume, _playVolume, time / fadeTimeInSeconds);
                time += Time.deltaTime;
                yield return null;
            }
            _audioSource.volume = _playVolume;
        }

        /// <summary>
        /// Fade out music source async
        /// </summary>
        /// <param name="clipIndex"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator FadeOutAsync(Action callBack)
        {
            float time = 0;
            while (time < fadeTimeInSeconds)
            {
                _audioSource.volume = Mathf.Lerp(_playVolume, _silentVolume, time / fadeTimeInSeconds);
                time += Time.deltaTime;
                yield return null;
            }
            _audioSource.volume = _silentVolume;

            // Call the callback function
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }
        #endregion
    }
}
