using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Effects
{
    [RequireComponent(typeof(AudioSource))] [RequireComponent(typeof(ParticleSystem))]
    public class Explosion : MonoBehaviour
    {
        [BoxGroup("Pooling Settings")] [SerializeField] private bool usePooling;
        [BoxGroup("Pooling Settings")] [SerializeField] private float returnToPoolDelay = 5.0f;
        [BoxGroup("Settings")] [SerializeField] private AudioClip[] audioClips;
        [FoldoutGroup("Events")] public UnityEvent<Explosion> ReturnToPoolEvent;

        private AudioSource _audioSource;
        private ParticleSystem _particleSystem;

        private int _numClips;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _particleSystem = GetComponent<ParticleSystem>();
            _audioSource.Stop();
            _particleSystem.Stop();

            _numClips = audioClips.Length;
        }

        /// <summary>
        /// Trigger explosion
        /// </summary>
        [Button("Explode!")]
        public void Explode(bool playSound)
        {
            if (playSound)
            {
                PlayRandomSound();
            }

            _particleSystem.Play(true);
            if (usePooling)
            {
                StartCoroutine(ReturnToPoolAsync());
            }
        }

        /// <summary>
        /// Reset when explosion is done or returned to the pool
        /// </summary>
        internal void ResetExplosion()
        {
            _particleSystem.Stop();
            _audioSource.Stop();
        }

        /// <summary>
        /// Return to the pool after delay
        /// </summary>
        private IEnumerator ReturnToPoolAsync()
        {
            yield return new WaitForSeconds(returnToPoolDelay);
            ReturnToPoolEvent.Invoke(this);
        }

        /// <summary>
        /// Play a random explosion sound
        /// </summary>
        [Button("Play Sound")]
        private void PlayRandomSound()
        {
            // Get a random clip
            int clipIndex = Random.Range(0, _numClips - 1);
            _audioSource.PlayOneShot(audioClips[clipIndex]);
        }
    }
}