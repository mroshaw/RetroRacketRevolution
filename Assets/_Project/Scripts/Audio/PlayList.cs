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
        public AudioClip[] audioClips;

        // Public properties
        public int NumberOfSongs => audioClips.Length;

        /// <summary>
        /// Returns the song at given index
        /// </summary>
        /// <param name="index"></param>
        public AudioClip GetSongAtIndex(int index)
        {
            return audioClips[index];
        }
    }
}
