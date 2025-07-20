using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Players
{
    public class Engine : MonoBehaviour
    {
        [BoxGroup("VFX")] [SerializeField] private ParticleSystem engineParticles;
        [BoxGroup("Audio")] [SerializeField] private AudioClip engineSound;
        [BoxGroup("Audio")] [SerializeField] private float fadeTime = 0.2f;
        private AudioSource _audioSource;

        private bool _isFiring;
        private Coroutine _fadeCoroutine;
        private float _fadeVelocity = 0f;
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            engineParticles.Stop();
			_audioSource.volume = 0f;
            _audioSource.Play();
        }

		[Button("Fire Engine")]
        public void FireEngine()
        {
            if (_isFiring)
            {
                return;
            }

            FadeInAudio();
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
            FadeOutAudio();
            _isFiring = false;
        }

        private void FadeInAudio()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            
            _fadeCoroutine = StartCoroutine(FadeAudio(1f));
        }
        
        private void FadeOutAudio()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeAudio(0f));
        }
        
        private IEnumerator FadeAudio(float endValue)
        {
            float startValue = _audioSource.volume;
            
            /*
            float timer = 0f;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                _audioSource.volume = Mathf.Lerp(startValue, endValue, timer / fadeTime);
                yield return null;
            }
            */
            while (!Mathf.Approximately(_audioSource.volume, endValue))
            {
                _audioSource.volume = Mathf.SmoothDamp(_audioSource.volume, endValue, ref _fadeVelocity, fadeTime);
                yield return null;
            }
            
            _audioSource.volume = endValue;
            
            yield return new WaitForSeconds(0.05f); // Give Unity's audio system a moment
        }
    }
}