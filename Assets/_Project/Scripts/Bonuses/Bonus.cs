using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public class Bonus : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private BonusType bonusType;
        [BoxGroup("Settings")] [SerializeField] private float duration;
        [BoxGroup("Settings")] [SerializeField] private float spawnDownForce = 100.0f;
        [BoxGroup("Audio")] [SerializeField] private AudioClip spawnAudioClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip collectAudioClip;

        [BoxGroup("Score Bonus")] [SerializeField] private int scoreToAdd;
        [BoxGroup("AddOn")] [SerializeField] private AddOnType addOnType;
        [BoxGroup("AddOn")] [SerializeField] private HardPointLocation hardPointLocation;
        [BoxGroup("Events")] [SerializeField] internal UnityEvent<Bonus> onDestroyed;
        [BoxGroup("Events")] [SerializeField] internal UnityEvent onSpawned;

        internal BonusManager MainBonusManager { get; set; }
        internal BonusType BonusType => bonusType;
        internal int ScoreToAdd => scoreToAdd;
        internal float Duration => duration;
        internal HardPointLocation HardPointLocation => hardPointLocation;

        private AudioSource _audioSource;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
        }

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
                DestroyBonus();
                return;
            }

            // Hit the bottom of the screen
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                DestroyBonus();
            }
        }

        /// <summary>
        /// Applies whatever bonus effect has been collected
        /// </summary>
        private void ApplyBonus(Collision other)
        {
            // Play bonus audio
            if (collectAudioClip)
            {
                _audioSource.PlayOneShot(collectAudioClip);
            }
            else
            {
                Debug.Log($"No collect audio clip for bonus: {bonusType}");
            }

            // If Random, spawn a random bonus
            if (bonusType == BonusType.Random)
            {
                MainBonusManager.SpawnRandomBonus();
                return;
            }

            // Check if collision is bonus recipient
            IBonusRecipient bonusRecipient = other.gameObject.GetComponent<IBonusRecipient>();
            bonusRecipient?.ApplyBonus(this);
        }

        internal void Spawn()
        {
            if (spawnAudioClip)
            {
                _audioSource.PlayOneShot(spawnAudioClip);
            }

            PushDown();
            onSpawned?.Invoke();
        }

        private void PushDown()
        {
            _rigidbody.AddForce(new Vector3(0, -spawnDownForce, 0), ForceMode.Impulse);
        }

        internal void DestroyBonus()
        {
            onDestroyed?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}