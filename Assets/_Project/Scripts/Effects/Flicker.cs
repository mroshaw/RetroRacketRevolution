using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Effects
{
    public class Flicker : MonoBehaviour
    {
        [BoxGroup("Settings")] public float fadeTime = 3.0f;
        [BoxGroup("Settings")] public bool flickerOnStart = true;

        private Material _material;
        private Color _color;
        private Color _startColor;
        private Color _targetColor;

        private bool _fadeIn;

        /// <summary>
        /// Init this component
        /// </summary>
        private void Awake()
        {
            _material = GetComponent<SpriteRenderer>().material;
            _color = _material.GetColor("_Color");
            _startColor = new Color(_color.r, _color.g, _color.b, 0);
            _targetColor = new Color(_color.r, _color.g, _color.b, 1);
        }

        /// <summary>
        /// Initalise components
        /// </summary>
        private void Start()
        {
            if (flickerOnStart)
            {
                FadeInNow();
            }
        }

        [Button("Test")]
        private void Test()
        {
            _material = GetComponent<SpriteRenderer>().material;
            _color = _material.GetColor("_Color");
            _startColor = new Color(_color.r, _color.g, _color.b, 0);
            _targetColor = new Color(_color.r, _color.g, _color.b, 1);

            FadeInNow();
        }

        [Button("Stop")]
        private void Stop()
        {
            StopCoroutine(FlickerAsync());
        }

        /// <summary>
        /// Public, sync wrapper to async function
        /// </summary>
        public void FadeInNow()
        {
            StartCoroutine(FlickerAsync());
        }

        /// <summary>
        /// Async fade color alpha
        /// </summary>
        /// <returns></returns>
        private IEnumerator FlickerAsync()
        {
            while (true)
            {
                // Flicker in
                float time = 0;
                while (time < fadeTime)
                {
                    _material.color = Color.Lerp(_startColor, _targetColor, time / fadeTime);
                    time += Time.deltaTime;
                    yield return null;
                }
                _material.SetColor("_Color", _targetColor);

                // Flicker out
                time = 0;
                while (time < fadeTime)
                {
                    _material.color = Color.Lerp(_targetColor, _startColor, time / fadeTime);
                    time += Time.deltaTime;
                    yield return null;
                }
                _material.SetColor("_Color", _startColor);
            }
        }

        /// <summary>
        /// Stop the cooroutine on destroy
        /// </summary>
        private void OnDestroy()
        {
            Stop();
        }
    }
}
