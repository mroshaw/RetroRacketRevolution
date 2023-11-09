using System.IO;
using DaftAppleGames.RetroRacketRevolution.Levels;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Levels
{
    public class LevelDataResources : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("General Settings")]
        public string setting1;
        
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
            
        }
    
        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
#if !UNITY_EDITOR
            UnpackAllLevels();
#endif
        }
        #endregion

        #region PublicMethods

        /// <summary>
        /// Force an overwrite of level data, e.g. for an upgrade
        /// </summary>
        public void UnpackLevelData()
        {
            // Unpack original levels
            UnpackResourceToTarget("LevelData", LevelDataExt.OgLevelDataPath);

            // Unpack original levels
            UnpackResourceToTarget("LevelData", LevelDataExt.OgLevelDataPath);
        }
        #endregion

        #region PrivateMethods
        private void UnpackAllLevels()
        {
            // Create Original Level Data Folders, if not there
            if (!Directory.Exists(LevelDataExt.OgLevelDataPath))
            {
                Debug.Log($"Creating OG LevelData folder: {LevelDataExt.OgLevelDataPath}");
                Directory.CreateDirectory(LevelDataExt.OgLevelDataPath);

                // Unpack original levels
                UnpackResourceToTarget("LevelData", LevelDataExt.OgLevelDataPath);
            }

            // Create Custom Level Data Folders, if not there
            if (!Directory.Exists(LevelDataExt.CustomLevelDataPath))
            {
                Debug.Log($"Creating OG LevelData folder: {LevelDataExt.CustomLevelDataPath}");
                Directory.CreateDirectory(LevelDataExt.CustomLevelDataPath);

                // Unpack custom levels
                UnpackResourceToTarget("CustomLevelData", LevelDataExt.CustomLevelDataPath);
            }
        }
        
        /// <summary>
        /// Unpack level resources from location to the target
        /// </summary>
        /// <param name="resourcePath"></param>
        /// <param name="targetPath"></param>
        private void UnpackResourceToTarget(string resourcePath, string targetPath)
        {
            TextAsset[] allLevels = Resources.LoadAll<TextAsset>(resourcePath);
            
            // Save each level to a JSON file in target
            foreach (TextAsset level in allLevels)
            {
                string targetFile = level.name + ".json";
                Debug.Log($"Unpacking level: {targetFile} to {Path.Join(targetPath, targetFile)}");
                using (StreamWriter outputFile = new StreamWriter(Path.Join(targetPath, targetFile), false))
                {
                    outputFile.WriteLine(level.ToString());
                }
            }
        }
        #endregion
    }
}
