using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public class Bonus : MonoBehaviour
    {
        [BoxGroup("Settings")] public BonusType bonusType;
        [BoxGroup("Settings")] public float duration;
        [BoxGroup("Audio")] public AudioClip spawnAudioClip;
        [BoxGroup("Audio")] public AudioClip collectAudioClip;
        [BoxGroup("Score Bonus")] public int scoreToAdd;
        [BoxGroup("AddOn")] public AddOnType addOnType;
        [BoxGroup("AddOn")] public HardPointLocation hardPointLocation;

        public BonusManager MainBonusManager { get; set; }

        private void OnCollisionEnter(Collision other)
        {
            if (other == null)
            {
                return;
            }

            // Caught by Player
            if (other.gameObject.CompareTag("Player"))
            {
                ApplyBonus(other);
                Destroy(this.gameObject);
                return;
            }

            // Hit the bottom of the screen
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Applies whatever bonus effect has been collected
        /// </summary>
        private void ApplyBonus(Collision other)
        {
            MainBonusManager.ApplyBonusEffect(this, other.gameObject);
        }
    }
}