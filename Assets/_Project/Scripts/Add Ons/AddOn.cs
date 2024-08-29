using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public abstract class AddOn : MonoBehaviour
    {
        public Player AttachedPlayer => _attachedPlayer;
        private Player _attachedPlayer;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public virtual void Awake()
        {
            Attach();
        }

        /// <summary>
        /// Set up the components
        /// </summary>
        private void Start()
        {

        }

        /// <summary>
        /// Attach the add-on
        /// </summary>
        private void Attach()
        {
            Player player = GetComponentInParent<Player>();
            if (player != null)
            {
                _attachedPlayer = player;
            }
        }

        /// <summary>
        /// Detach the add-on
        /// </summary>
        private void Detach()
        {

        }

        /// <summary>
        /// Deploy add-on functionality
        /// </summary>
        public virtual void Deploy()
        {
        }

        /// <summary>
        /// Deploy add-on functionality
        /// </summary>
        public virtual void Retract()
        {
        }

    }
}
