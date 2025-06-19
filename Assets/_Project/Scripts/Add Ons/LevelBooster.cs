using System;
using System.Collections;
using DaftAppleGames.Players;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public class LevelBooster : AddOn
    {
        [BoxGroup("Settings")] [SerializeField] float boostTime = 3.0f;
        [BoxGroup("Settings")] [SerializeField] float boostHeight = 150.0f;
        [BoxGroup("Settings")] [SerializeField] private Engine engine1;
        [BoxGroup("Settings")] [SerializeField] private Engine engine2;

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
        internal void Awake()
        {
            _parentGameObject = GetComponentInParent<Player>().gameObject;
            _startHeight = this._parentGameObject.transform.position.y;
            _startHorizontal = this._parentGameObject.transform.position.x;
            _startPosition = new Vector2(_startHorizontal, _startHeight);
            _endPosition = new Vector2(_startHorizontal, boostHeight);
        }

        internal override void Fire()
        {
        }

        internal override void StopFire()
        {
        }

        /// <summary>
        /// Deploy override
        /// </summary>
        protected internal override void Deploy(Action callBack, bool immediate = false)
        {
            engine1.gameObject.SetActive(true);
            engine2.gameObject.SetActive(true);
            Boost();
            callBack?.Invoke();
        }

        protected internal override void Retract(Action callBack, bool immediate = false)
        {
            engine1.gameObject.SetActive(false);
            engine2.gameObject.SetActive(false);
            callBack?.Invoke();
        }

        /// <summary>
        /// Call the boost function
        /// </summary>
        public void Boost()
        {
            engine1.FireEngine();
            engine2.FireEngine();
            StartCoroutine(BoostAsync());
        }

        private void EndBoost()
        {
            engine1.StopFiringEngine();
            engine2.StopFiringEngine();
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