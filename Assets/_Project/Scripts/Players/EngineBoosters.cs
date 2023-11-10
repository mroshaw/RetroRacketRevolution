using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using static UnityEngine.ParticleSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class EngineBoosters : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public ParticleSystem leftBooster;
        [BoxGroup("General Settings")] public ParticleSystem rightBooster;
        [BoxGroup("General Settings")] public int maxParticleEmission = 200;

        // Private fields
        private EmissionModule _leftEmission;
        private EmissionModule _rightEmission;

        private bool _leftBoostFiring;
        private bool _rightBoostFiring;

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _leftEmission = leftBooster.emission;
            _rightEmission = rightBooster.emission;
            _leftBoostFiring = false;
            _rightBoostFiring = false;

            NoBoost();

            leftBooster.Play();
            rightBooster.Play();
        }
        #endregion

        /// <summary>
        /// Trigger left booster
        /// </summary>
        public void LeftBoost()
        {
            if (!_leftBoostFiring)
            {
                _leftBoostFiring = true;
                _rightBoostFiring = false;

                _leftEmission.enabled = true;
                _rightEmission.enabled = false;

                _leftEmission.rateOverTime = maxParticleEmission;
                _rightEmission.rateOverTime = 0;
            }
        }

        /// <summary>
        /// Trigger right booster
        /// </summary>
        public void RightBoost()
        {
            if (!_rightBoostFiring)
            {
                _rightEmission.enabled = true;
                _leftEmission.enabled = false;

                _rightBoostFiring = true;
                _leftBoostFiring = false;
                _leftEmission.rateOverTime = 0;
                _rightEmission.rateOverTime = maxParticleEmission;
            }
        }

        /// <summary>
        /// No boosters firing
        /// </summary>
        public void NoBoost()
        {
            _rightBoostFiring = false;
            _leftBoostFiring = false;
            _leftEmission.rateOverTime = 0;
            _rightEmission.rateOverTime = 0;

            _leftEmission.enabled = false;
            _rightEmission.enabled = false;
        }
    }
}
