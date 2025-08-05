using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public abstract class AddOn : MonoBehaviour
    {
        [BoxGroup("Debug")] [SerializeField] private Player attachedPlayer;
        [BoxGroup("Debug")] [SerializeField] private HardPoint attachedHardPoint;

        public Player AttachedPlayer => attachedPlayer;
        public HardPoint AttachedHardPoint => attachedHardPoint;

        protected bool IsDeployed => attachedHardPoint && attachedHardPoint.IsDeployed;

        protected AudioSource AudioSource;

        internal abstract void Fire();
        internal abstract void StopFire();

        protected virtual void Awake()
        {
            AudioSource = GetComponent<AudioSource>();
        }

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

            attachedHardPoint = hardPoint;
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
        /// Async routines for specific Addons
        /// </summary>
        protected internal abstract IEnumerator Deploy(bool immediate = false);

        protected internal abstract IEnumerator Retract(bool immediate = false);
    }
}