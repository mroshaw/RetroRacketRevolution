using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class LevelBooster : AddOn
    {
        [BoxGroup("Booster Settings")] public float boostTime = 3.0f;
        [BoxGroup("Booster Settings")] public float boostHeight = 150.0f;

        [FoldoutGroup("Events")] public UnityEvent StartBoostEvent;
        [FoldoutGroup("Events")] public UnityEvent EndBoostEvent;

        private float _startHeight;
        private float _startHorizontal;

        private GameObject _parentGameObject;

        private Vector2 _startPosition;
        private Vector2 _endPosition;

        private AudioSource _audioSource;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _parentGameObject = GetComponentInParent<Player>().gameObject;
            _audioSource = GetComponent<AudioSource>();
            _startHeight = this._parentGameObject.transform.position.y;
            _startHorizontal = this._parentGameObject.transform.position.x;
            _startPosition = new Vector2(_startHorizontal, _startHeight);
            _endPosition = new Vector2(_startHorizontal, boostHeight);
        }

        /// <summary>
        /// Deploy override
        /// </summary>
        public override void Deploy()
        {
            Boost();
        }

        /// <summary>
        /// Call the boost function
        /// </summary>
        public void Boost()
        {
            _audioSource.Play();
            StartCoroutine(BoostAsync());
        }

        /// <summary>
        /// Boost up the screen, async
        /// </summary>
        /// <returns></returns>
        private IEnumerator BoostAsync()
        {
            StartBoostEvent.Invoke();
            yield return null;
            float time = 0;
            while (time < boostTime)
            {
                _parentGameObject.transform.position = Vector2.Lerp(_startPosition, _endPosition, time / boostTime);
                time += Time.deltaTime;
                yield return null;
            }
            _parentGameObject.transform.position = _endPosition;
            EndBoostEvent.Invoke();
        }
    }
}
