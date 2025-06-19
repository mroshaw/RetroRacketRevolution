using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public enum DeploymentState { Deploying, Deployed, Retracting, Retracted }

    public class HardPoint : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private bool deployOnStart;
        [BoxGroup("Settings")] [SerializeField] private AddOn defaultAddOn;
        [BoxGroup("Debug")] [SerializeField] private AddOn attachedAddOn;
        [BoxGroup("Debug")] [SerializeField] private DeploymentState deploymentState;

        [FoldoutGroup("Events")] public UnityEvent deployEvent;
        [FoldoutGroup("Events")] public UnityEvent retractEvent;

        internal bool IsDeployed => deploymentState == DeploymentState.Deployed;
        internal DeploymentState DeploymentState => deploymentState;

        internal Player HardPointPlayer => _hardPointPlayer;


        private bool IsHardPointOccupied => attachedAddOn != null;
        private Player _hardPointPlayer;

        /// <summary>
        /// Set up the Hard Point
        /// </summary>
        private void Awake()
        {
            _hardPointPlayer = GetComponentInParent<Player>();

            if (defaultAddOn)
            {
                AttachAddOn(defaultAddOn, true);
            }

            if (!IsHardPointOccupied)
            {
                return;
            }

            if (deployOnStart)
            {
                Deploy(true);
            }
            else
            {
                Retract(true);
            }
        }

        internal void AttachAddOn(AddOn addOn, bool forceReplace)
        {
            if (attachedAddOn == addOn || (IsHardPointOccupied && !forceReplace))
            {
                return;
            }

            attachedAddOn = addOn;
            addOn.Attach(this);
        }

        internal void DetachAddOn()
        {
            if (!IsHardPointOccupied)
            {
                attachedAddOn.Detach();
            }
        }

        /// <summary>
        /// Setup the component
        /// </summary>
        private void Start()
        {
            if (deployOnStart)
            {
                Deploy();
            }
            else
            {
                Retract();
            }
        }

        public void FirePressed()
        {
            if (deploymentState != DeploymentState.Deployed)
            {
                return;
            }

            attachedAddOn.Fire();
        }

        public void FireReleased()
        {
            attachedAddOn.StopFire();
        }


        /// <summary>
        /// Deploys the hardpoint
        /// </summary>
        [Button("Deploy")]
        public void Deploy(bool immediate = false)
        {
            if (deploymentState != DeploymentState.Retracted || !IsHardPointOccupied)
            {
                return;
            }
            deploymentState = DeploymentState.Deploying;
            attachedAddOn.Deploy(DeployedCallBack, immediate);
            deployEvent.Invoke();
        }

        private void DeployedCallBack()
        {
            deploymentState = DeploymentState.Deployed;
        }

        /// <summary>
        /// Retracts the hardpoint
        /// </summary>
        [Button("Retract")]
        public void Retract(bool immediate = false)
        {
            if (deploymentState != DeploymentState.Deployed || !IsHardPointOccupied)
            {
                return;
            }
            deploymentState = DeploymentState.Retracting;
            attachedAddOn.Retract(RetractCallBack, immediate);
            retractEvent.Invoke();
        }

        private void RetractCallBack()
        {
            deploymentState = DeploymentState.Retracted;
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
            Retract();
        }
    }
}