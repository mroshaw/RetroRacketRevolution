using System.IO;
using DaftAppleGames.RetroRacketRevolution.Levels;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.Levels
{
    public class LevelDataResources : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private bool initOnStart; 
        [BoxGroup("Events")] public UnityEvent filesUnpackedEvent;

        private void Start()
        {
            if(initOnStart)
            {
                UnpackAllLevels();
            }
        }

        /// <summary>
        /// Force an overwrite of level data, e.g. for an upgrade
        /// </summary>
        public void UnpackLevelData()
        {
            // Unpack original levels
            UnpackResourceToTarget("LevelData", LevelDataExt.OgLevelDataPath, true);

            // Unpack custom levels
            UnpackResourceToTarget("CustomLevelData", LevelDataExt.CustomLevelDataPath, true);

            filesUnpackedEvent.Invoke();
        }

        public void UnpackAllLevels()
        {
            // Create Original Level Data Folders, if not there
            if (!Directory.Exists(LevelDataExt.OgLevelDataPath))
            {
                Debug.Log($"Creating OG LevelData folder: {LevelDataExt.OgLevelDataPath}");
                Directory.CreateDirectory(LevelDataExt.OgLevelDataPath);

                // Unpack original levels
                UnpackResourceToTarget("LevelData", LevelDataExt.OgLevelDataPath, false);
            }

            // Create Custom Level Data Folders, if not there
            if (!Directory.Exists(LevelDataExt.CustomLevelDataPath))
            {
                Debug.Log($"Creating OG LevelData folder: {LevelDataExt.CustomLevelDataPath}");
                Directory.CreateDirectory(LevelDataExt.CustomLevelDataPath);

                // Unpack custom levels
                UnpackResourceToTarget("CustomLevelData", LevelDataExt.CustomLevelDataPath, false);
            }

            filesUnpackedEvent.Invoke();
        }
        
        /// <summary>
        /// Unpack level resources from location to the target
        /// </summary>
        private void UnpackResourceToTarget(string resourcePath, string targetPath, bool overwrite)
        {
            TextAsset[] allLevels = Resources.LoadAll<TextAsset>(resourcePath);
            
            // Save each level to a JSON file in target
            foreach (TextAsset level in allLevels)
            {
                string targetFile = level.name + ".json";
                if (overwrite || !File.Exists(targetFile))
                {
                    Debug.Log($"Unpacking level: {targetFile} to {Path.Join(targetPath, targetFile)}");
                    using StreamWriter outputFile = new StreamWriter(Path.Join(targetPath, targetFile), false);
                    outputFile.WriteLine(level.ToString());
                }
            }
        }
    }
}