using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Platform
{
    public class VersionActions : MonoBehaviour
    {
        // Public serializable properties
        [FoldoutGroup("No Version Events")]
        public UnityEvent NoVersionFoundEvent;
        [FoldoutGroup("Version Before Events")]
        public UnityEvent InstalledVersionNewerEvent;
        [FoldoutGroup("Version After Events")]
        public UnityEvent InstalledVersionOlderEvent;

        // Public properties

        // Private fields

        #region UnityMethods
        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            ProcessApplicationVersion();
        }
        #endregion

        /// <summary>
        /// Determine what to do
        /// </summary>
        private void ProcessApplicationVersion()
        {
            string currVersionString = Application.version;
            Version currVersion = Version.Parse(currVersionString);

            string installedVersionString = GetInstalledVersion();

            if (string.IsNullOrEmpty(installedVersionString))
            {
                Debug.Log("No version found");
                NoVersionFoundEvent.Invoke();
                return;
            }

            Version installedVersion = Version.Parse(installedVersionString);
            if (currVersion.CompareTo(installedVersion) < 0)
            {
                Debug.Log("Installed version is older");
                InstalledVersionOlderEvent.Invoke();
                return;
            }

            if (currVersion.CompareTo(installedVersion) > 0)
            {
                Debug.Log("Installed version is newer");
                InstalledVersionNewerEvent.Invoke();
                return;
            }
        }

        /// <summary>
        /// Get the Installed Version
        /// </summary>
        /// <returns></returns>
        public string GetInstalledVersion()
        {
            return PlayerPrefs.GetString("InstalledVersion", String.Empty);
        }

        /// <summary>
        /// Set the Installed Version
        /// </summary>
        public void SetInstalledVersion()
        {
            PlayerPrefs.SetString("InstalledVersion", Application.version);
        }
    }
}
