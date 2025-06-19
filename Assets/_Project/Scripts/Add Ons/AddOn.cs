using System;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public abstract class AddOn : MonoBehaviour
    {
        [BoxGroup("Debug")] [SerializeField] private Player attachedPlayer;
        [BoxGroup("Debug")] [SerializeField] private HardPoint attachedHardPoint;

        public Player AttachedPlayer => attachedPlayer;
        public HardPoint AttachedHardPoint => attachedHardPoint;

        protected bool IsDeployed => attachedHardPoint && attachedHardPoint.IsDeployed;

        internal abstract void Fire();
        internal abstract void StopFire();

        /// <summary>
        /// Attach the add-on
        /// </summary>
        internal void Attach(HardPoint hardPoint)
        {
            Player player = GetComponentInParent<Player>();
            if (player != null)
            {
                attachedPlayer = player;
            }

            attachedHardPoint =  hardPoint;
        }

        /// <summary>
        /// Detach the add-on
        /// </summary>
        internal void Detach()
        {
            attachedPlayer = null;
            attachedHardPoint = null;
        }

        /// <summary>
        /// Deploy add-on functionality
        /// </summary>
        protected internal abstract void Deploy(Action callBack, bool immediate = false);

        /// <summary>
        /// Deploy add-on functionality
        /// </summary>
        protected internal abstract void Retract(Action callBack, bool immediate = false);
    }
}