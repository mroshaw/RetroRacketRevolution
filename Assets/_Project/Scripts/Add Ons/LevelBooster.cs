using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public class LevelBooster : AddOn
    {
        [BoxGroup("Settings")] [SerializeField] private float boostTime = 3.0f;
        [BoxGroup("Settings")] [SerializeField] private Vector3 boostVector = new Vector3(0, 150.0f, -100.0f);
        [BoxGroup("Settings")] [SerializeField] private Engine engine1;
        [BoxGroup("Settings")] [SerializeField] private Engine engine2;

        [BoxGroup("Deployment")] [SerializeField] private Vector3 deployedPosition;
        [BoxGroup("Deployment")] [SerializeField] private Vector3 retractedPosition;
        [BoxGroup("Deployment")] [SerializeField] private float deployTime = 0.5f;

        [BoxGroup("Events")] public UnityEvent onStartBoost;
        [BoxGroup("Events")] public UnityEvent onEndBoost;

        private GameObject _parentGameObject;
        private AudioSource _audioSource;

        protected override void Awake()
        {
            base.Awake();
            _parentGameObject = GetComponentInParent<Player>().gameObject;
        }

        internal override void Fire()
        {
        }

        internal override void StopFire()
        {
        }

        protected internal override IEnumerator Deploy(bool immediate = false)
        {
            yield return MoveBooster(retractedPosition, deployedPosition, immediate ? 0.0f : deployTime);
            yield return BoostAsync();
        }

        protected internal override IEnumerator Retract(bool immediate = false)
        {
            yield return MoveBooster(deployedPosition, retractedPosition, immediate ? 0.0f : deployTime);
        }

        private IEnumerator MoveBooster(Vector3 startPosition, Vector3 endPosition, float moveTime)
        {
            float elapsedTime = 0;
            while (elapsedTime < moveTime)
            {
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = endPosition;
        }

        private void ResetBoosters()
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
            engine1.FireEngine();
            engine2.FireEngine();
            Vector3 startPosition = _parentGameObject.transform.position;
            Vector3 endPosition = startPosition + boostVector;

            onStartBoost.Invoke();
            yield return null;
            float time = 0;
            while (time < boostTime)
            {
                _parentGameObject.transform.position = Vector3.Lerp(startPosition, endPosition, time / boostTime);
                time += Time.deltaTime;
                yield return null;
            }

            _parentGameObject.transform.position = endPosition;
            ResetBoosters();
            Debug.Log("Boost Complete!");
            onEndBoost.Invoke();
        }
    }
}