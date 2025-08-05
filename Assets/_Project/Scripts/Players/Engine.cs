using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class Engine : MonoBehaviour
    {
        [BoxGroup("VFX")] [SerializeField] private ParticleSystem engineParticles;
        [BoxGroup("Audio")] [SerializeField] private AudioClip engineSound;
        [BoxGroup("Audio")] [SerializeField] private float maxVolume = 0.6f;
        [BoxGroup("Audio")] [SerializeField] private float audioModifier = 1.0f;

        private AudioSource _audioSource;

        private bool _isFiring;
        private bool _canFire;

        private void OnEnable()
        {
            _canFire = true;
            _audioSource.volume = 0f;
            _audioSource.Play();
        }

        private void OnDisable()
        {
            _canFire = false;
            _audioSource.Stop();
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            engineParticles.Stop();
            _audioSource.clip = engineSound;
        }

        private void Update()
        {
            if (!_canFire ||
                (_isFiring && _audioSource.volume >= maxVolume) ||
                (!_isFiring && _audioSource.volume <= 0))
            {
                return;
            }

            if (_isFiring)
            {
                _audioSource.volume += Time.deltaTime * audioModifier;
            }
            else
            {
                _audioSource.volume -= Time.deltaTime * audioModifier;
            }
        }

        [Button("Fire Engine")]
        public void FireEngine()
        {
            if (_isFiring)
            {
                return;
            }

            engineParticles.Play();
            _isFiring = true;
        }

        [Button("Stop Firing Engine")]
        public void StopFiringEngine()
        {
            if (!_isFiring)
            {
                return;
            }

            engineParticles.Stop();
            _isFiring = false;
        }
    }
}