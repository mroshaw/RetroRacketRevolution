using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class FadeIn : MonoBehaviour
    {
        [BoxGroup("Settings")] public float fadeInTime = 3.0f;
        [BoxGroup("Settings")] public bool fadeOnStart = true;

        private Material _material;
        private Color _color;
        private Color _startColor;
        private Color _targetColor;

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
            if (fadeOnStart)
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

        /// <summary>
        /// Public, sync wrapper to async function
        /// </summary>
        public void FadeInNow()
        {
            StartCoroutine(FadeInAsync());
        }

        /// <summary>
        /// Async fade color alpha
        /// </summary>
        /// <returns></returns>
        private IEnumerator FadeInAsync()
        {
            float time = 0;
            while (time < fadeInTime)
            {
                _material.color = Color.Lerp(_startColor, _targetColor, time / fadeInTime);
                time += Time.deltaTime;
                yield return null;
            }
            _material.SetColor("_Color", _targetColor);
        }
    }
}
