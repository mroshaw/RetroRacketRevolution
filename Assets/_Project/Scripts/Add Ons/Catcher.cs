using System;
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
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        internal override void Fire()
        {
            if (!_isCaught)
            {
                return;
            }
            AttachedPlayer.Fire();
            _ball = null;
        }

        internal override void StopFire()
        {
        }

        protected internal override void Deploy(Action callBack, bool immediate = false)
        {
        }

        protected internal override void Retract(Action callBack, bool immediate = false)
        {
  }

        /// <summary>
        /// Ball enters catcher trigger collider
        /// </summary>
        /// <param name="other"></param>
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