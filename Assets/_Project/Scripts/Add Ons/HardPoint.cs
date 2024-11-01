using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class HardPoint : MonoBehaviour
    {
        [BoxGroup("Settings")] public bool deployOnStart = false;
        [BoxGroup("Settings")] public AddOn attachedAddOn;

        [FoldoutGroup("Events")] public UnityEvent DeployEvent;
        [FoldoutGroup("Events")] public UnityEvent RetractEvent;

        public bool IsHardPointOccupied => attachedAddOn != null;
        public bool IsHardPointEnabled => attachedAddOn.gameObject.activeSelf;
        public Player HardPointPlayer => _hardPointPlayer;

        private Player _hardPointPlayer;

        private bool _isDeployed;

        /// <summary>
        /// Set up the Hard Point
        /// </summary>
        private void Awake()
        {
            _hardPointPlayer = GetComponentInParent<Player>();
        }

        /// <summary>
        /// Setup the component
        /// </summary>
        private void Start()
        {
            if (deployOnStart)
            {
                EnableAddOn();
            }
            else
            {
                DisableAddOn();
            }
        }

        /// <summary>
        /// Enable attached add-on
        /// </summary>
        public void EnableAddOn()
        {
            attachedAddOn.gameObject.SetActive(true);
            Deploy();
        }

        /// <summary>
        /// Disable the add-on
        /// </summary>
        public void DisableAddOn()
        {
            attachedAddOn.gameObject.SetActive(false);
            Retract();
        }

        /// <summary>
        /// Deploys the hardpoint
        /// </summary>
        public void Deploy()
        {
            if (_isDeployed)
            {
                return;
            }
            DeployEvent.Invoke();
            _isDeployed = true;
        }

        /// <summary>
        /// Retracts the hardpoint
        /// </summary>
        public void Retract()
        {
            if (!_isDeployed)
            {
                return;
            }
            RetractEvent.Invoke();
            _isDeployed = false;
        }

        /// <summary>
        /// Wrapper for async method to remove the add-on
        /// </summary>
        /// <param name="delay"></param>
        public void DeactivateHardPointAfterDelay(float delay)
        {
            StartCoroutine(DeactivateHardPointAfterDelayAsync(delay));
        }

        /// <summary>
        /// Removes add-on from hardpoint after a given delay
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator DeactivateHardPointAfterDelayAsync(float delay)
        {
            yield return new WaitForSeconds(delay);
            // hardPoint.DetachAddOn(true, true);
            DisableAddOn();
        }
    }
}
