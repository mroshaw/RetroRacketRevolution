using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Explosion : MonoBehaviour
    {
        [BoxGroup("Settings")] public float returnToPoolDelay = 5.0f;
        [FoldoutGroup("Events")] public UnityEvent<Explosion> ReturnToPoolEvent;

        private AudioSource _audioSource;
        private ParticleSystem _particleSystem;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _particleSystem = GetComponent<ParticleSystem>();

            _particleSystem.Stop();
        }

        private void Start()
        {
        }

        /// <summary>
        /// Trigger explosion
        /// </summary>
        public void Explode()
        {
            if (_audioSource != null && _particleSystem != null)
            {
                _audioSource.Play();
                _particleSystem.Emit(100);
                StartCoroutine(ReturnToPoolAsync());
            }
        }

        /// <summary>
        /// Return to the pool after delay
        /// </summary>
        /// <returns></returns>
        private IEnumerator ReturnToPoolAsync()
        {
            yield return new WaitForSeconds(returnToPoolDelay);
            _particleSystem.Stop();
            ReturnToPoolEvent.Invoke(this);
        }
    }
}
