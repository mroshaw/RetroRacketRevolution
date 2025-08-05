using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Platform
{
    public class PlatformObjects : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("Settings")] [SerializeField] private PlatformObject[] platformObjects;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            ApplyPlatform();
        }

        /// <summary>
        /// Apply settings to current runtime platform
        /// </summary>
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
        private void SetGameObjectsState(GameObject[] gameObjects, bool state)
        {
            foreach (GameObject currentGameObject in gameObjects)
            {
                currentGameObject.SetActive(state);
            }
        }

        [Serializable] public class PlatformObject
        {
            public RuntimePlatform platform;
            public GameObject[] objectsToEnable;
            public GameObject[] objectsToDisable;
            public UnityEvent PlatformEvent;
        }
    }
}