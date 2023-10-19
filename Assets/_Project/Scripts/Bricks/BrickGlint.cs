using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class BrickGlint : MonoBehaviour
    {
        [BoxGroup("Settings")] public float glintDuration = 1.0f;
        [BoxGroup("Settings")] public float timeBetweenGlints = 2.0f;
        [BoxGroup("Settings")] public float startPos = -3.0f;
        [BoxGroup("Settings")] public float endPos = 4.6f;

        [BoxGroup("Settings")] public SpriteRenderer glintSpriteRenderer;

        [BoxGroup("Debug")] [SerializeField] private float _nextGlintTime;
        [BoxGroup("Debug")] [SerializeField] private bool _inGlint;
        [BoxGroup("Debug")] [SerializeField] private Vector2 _startPos;
        [BoxGroup("Debug")] [SerializeField] private Vector2 _endPos;

        /// <summary>
        /// Initialise component
        /// </summary>
        private void Awake()
        {
            _startPos = new Vector2(startPos, 0);
            _endPos = new Vector2(endPos, 0);
            Reset();
        }

        /// <summary>
        /// Check if it's time to glint, then glint
        /// </summary>
        private void Update()
        {
            Glint();
        }

        /// <summary>
        /// Resets the glint state
        /// </summary>
        public void Reset()
        {
            _nextGlintTime = 0.0f;
            _inGlint = false;
            _nextGlintTime = Time.time + timeBetweenGlints;
        }

        /// <summary>
        /// Glint at regular intervals over time
        /// </summary>
        private void Glint()
        {
            if (_inGlint)
            {
                return;
            }
            // Time to glint?
            if (Time.time > _nextGlintTime)
            {
                StartCoroutine(GlintAsync());
            }
        }

        /// <summary>
        /// Move the glint over the mask over time
        /// </summary>
        /// <returns></returns>
        private IEnumerator GlintAsync()
        {
            _inGlint = true;

            float time = 0;

            while (time < glintDuration)
            {
                glintSpriteRenderer.gameObject.transform.localPosition =
                    Vector2.Lerp(_startPos, _endPos, time / glintDuration);
                time += Time.deltaTime;
                yield return null;
            }

            _nextGlintTime = Time.time + timeBetweenGlints;
            yield return null;
            _inGlint = false;
        }
    }
}
