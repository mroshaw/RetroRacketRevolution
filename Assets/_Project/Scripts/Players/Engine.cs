using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using static UnityEngine.ParticleSystem;

namespace DaftAppleGames.Players
{
    public class Engine : MonoBehaviour
    {
        [BoxGroup("VFX")] [SerializeField] private ParticleSystem engineParticles;
        [BoxGroup("VFX")] [SerializeField] private int maxParticleEmission = 200;
        [BoxGroup("Audio")] [SerializeField] private AudioClip engineSound;

        private EmissionModule _emission;
        private AudioSource _audioSource;

        private bool _isFiring;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _emission = engineParticles.emission;
            engineParticles.Stop();
        }

        public void FireEngine()
        {
            if (_isFiring)
            {
                return;
            }
            // _emission.rateOverTime = maxParticleEmission;
            _audioSource.Play();
            engineParticles.Play();
            _isFiring = true;
        }

        public void StopFiringEngine()
        {
            if (!_isFiring)
            {
                return;
            }
            // _emission.rateOverTime = 0;
            engineParticles.Stop();
            _audioSource.Stop();
            _isFiring = false;
        }
    }
}