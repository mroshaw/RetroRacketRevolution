using System;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public abstract class AddOn : MonoBehaviour
    {

        [BoxGroup("Audio")] [SerializeField] private AudioClip deployClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip retractClip;

        [BoxGroup("Debug")] [SerializeField] private Player attachedPlayer;
        [BoxGroup("Debug")] [SerializeField] private HardPoint attachedHardPoint;

        public Player AttachedPlayer => attachedPlayer;
        public HardPoint AttachedHardPoint => attachedHardPoint;

        protected bool IsDeployed => attachedHardPoint && attachedHardPoint.IsDeployed;

        protected internal AudioSource AudioSource;

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
        protected internal virtual void Deploy(Action callBack, bool immediate = false)
        {
            if (AudioSource && deployClip)
            {
                AudioSource.PlayOneShot(deployClip);
            }
        }

        /// <summary>
        /// Deploy add-on functionality
        /// </summary>
        protected internal virtual void Retract(Action callBack, bool immediate = false)
        {
            if (AudioSource && retractClip)
            {
                AudioSource.PlayOneShot(retractClip);
            }
        }
    }
}