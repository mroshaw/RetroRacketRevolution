using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Platform
{
    public class PlatformObjects : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("Settings")] public PlatformObject[] platformObjects;
        
        [FoldoutGroup("Events")]
        public UnityEvent MyEvent;

        // Public properties
        
        // Private fields

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            ApplyPlatform();
        }
    
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            
        }

        /// <summary>
        /// Clean up on Destroy
        /// </summary>
        private void OnDestroy()
        {
            
        }
        #endregion

        #region PublicMethods

        #endregion

        #region PrivateMethods
        /// <summary>
        /// Apply settings to current runtime platform
        /// </summary>
        /// <param name="platform"></param>
        private void ApplyPlatform()
        {
            foreach (PlatformObject platformObject in platformObjects)
            {
                if (Application.platform == platformObject.platform)
                {
                    SetGameObjectsState(platformObject.objectsToDisable, false);
                    SetGameObjectsState(platformObject.objectsToEnable, true);
                    platformObject.PlatformEvent.Invoke();
                }
            }
        }

        /// <summary>
        /// Set the GameObject list to the given state
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="state"></param>
        private void SetGameObjectsState(GameObject[] gameObjects, bool state)
        {
            foreach (GameObject currentGameObject in gameObjects)
            {
                currentGameObject.SetActive(state);
            }
        }
        #endregion

        [Serializable]
        public class PlatformObject
        {
            public RuntimePlatform platform;
            public GameObject[] objectsToEnable;
            public GameObject[] objectsToDisable;
            public UnityEvent PlatformEvent;
        }
    }
}
