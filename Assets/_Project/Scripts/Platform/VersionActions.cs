using System;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Platform
{
    public class VersionActions : MonoBehaviour
    {
        // Public serializable properties
        [FoldoutGroup("No Version Events")] public UnityEvent onNoVersionFound;
        [FoldoutGroup("Version Before Events")] public UnityEvent onNewerVersionInstalled;
        [FoldoutGroup("Version After Events")] public UnityEvent onOlderVersionInstalled;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            ProcessApplicationVersion();
        }

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
                onNoVersionFound.Invoke();
                return;
            }

            Version installedVersion = Version.Parse(installedVersionString);
            if (currVersion.CompareTo(installedVersion) < 0)
            {
                Debug.Log("Installed version is older");
                onOlderVersionInstalled.Invoke();
                return;
            }

            if (currVersion.CompareTo(installedVersion) > 0)
            {
                Debug.Log("Installed version is newer");
                onNewerVersionInstalled.Invoke();
                return;
            }
        }

        /// <summary>
        /// Get the Installed Version
        /// </summary>
        /// <returns></returns>
        private string GetInstalledVersion()
        {
            return PlayerPrefs.GetString("InstalledVersion", String.Empty);
        }

        /// <summary>
        /// Set the Installed Version
        /// </summary>
        private void SetInstalledVersion()
        {
            PlayerPrefs.SetString("InstalledVersion", Application.version);
        }
    }
}