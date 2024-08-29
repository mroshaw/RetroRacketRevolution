using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Audio
{
    /// <summary>
    /// Playlist for music player
    /// </summary>
    [CreateAssetMenu(fileName = "PlayList", menuName = "Audio/Play List", order = 1)]
    public class PlayList : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("General Settings")]
        public PlayListSong[] Songs;

        // Public properties
        public int NumberOfSongs => Songs.Length;

        /// <summary>
        /// Returns the song at given index
        /// </summary>
        /// <param name="index"></param>
        public AudioClip GetAudioClipAtIndex(int index)
        {
            return Songs[index].AudioClip;
        }

        [Serializable]
        public class PlayListSong
        {
            public AudioClip AudioClip;
            public string SongName;
        }
    }
}
