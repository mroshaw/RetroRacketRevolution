using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Balls;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public class Catcher : AddOn
    {
        private Collider _collider;
        private Ball _ball;
        private bool _isCaught;

        /// <summary>
        /// Initialise this component
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
        }

        internal override void Fire()
        {
            if (!_isCaught)
            {
                return;
            }

            AttachedPlayer.BeginFiring();
            _ball = null;
        }

        internal override void StopFire()
        {
        }

        protected internal override IEnumerator Deploy(bool immediate = false)
        {
            yield break;
        }

        protected internal override IEnumerator Retract(bool immediate = false)
        {
            yield break;
        }

        /// <summary>
        /// Ball enters catcher trigger collider
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ball"))
            {
                _ball = other.GetComponent<Ball>();
                _ball.Attach(AttachedPlayer, _collider.bounds.ClosestPoint(other.transform.position));
                _isCaught = true;
            }
        }
    }
}