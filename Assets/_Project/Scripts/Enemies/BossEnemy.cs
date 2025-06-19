using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.Enemies
{
    public class BossEnemy : Enemy
    {
        // Public serializable properties[FoldoutGroup("Events")]
        public UnityEvent MyEvent;

        // Public properties
        
        // Private fields

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Configure the component on Start
        /// </summary>
        internal override void Start()
        {
            base.Start();
        }
        #endregion


        #region PublicMethods
        public override void OnSpawn()
        {
            base.OnSpawn();
        }
        #endregion

        #region PrivateMethods

        #endregion
    }
}