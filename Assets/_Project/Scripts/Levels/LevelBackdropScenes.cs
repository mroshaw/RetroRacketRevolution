using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Profile;
#endif

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    [CreateAssetMenu(fileName = "LevelBackdrops", menuName = "Level Editor/Level Backdrops", order = 1)]
    public class LevelBackdropScenes : ScriptableObject
    {
        [SerializeField] private List<BackdropScene> backdropScenes;

        /// <summary>
        /// Return the scene by index
        /// </summary>
        public string GetScene(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= backdropScenes.Count)
            {
                Debug.LogError($"LevelBackdropScenes: scene index {sceneIndex} is out of range!");
                return backdropScenes[0].SceneName;
            }

            return backdropScenes[sceneIndex].SceneName;
        }

        /// <summary>
        /// Get a list of scene names
        /// </summary>
        public List<string> GetSceneNames()
        {
            List<string> sceneNamesList = new List<string>();
            foreach (BackdropScene backdropScene in backdropScenes)
            {
                sceneNamesList.Add(backdropScene.SceneName);
            }

            return sceneNamesList;
        }

#if UNITY_EDITOR

        [Button("Sync to Build Profile (Windows)")]
        private void SyncScenesToBuildProfile()
        {
            BuildProfile specificBuildProfile = AssetDatabase.LoadAssetAtPath<BuildProfile>(
                "Assets/Settings/Build Profiles/Windows.asset"
            );
            BuildProfile.SetActiveBuildProfile(specificBuildProfile);

            // Get active build profile
            BuildProfile activeProfile = BuildProfile.GetActiveBuildProfile();
            if (activeProfile == null)
            {
                Debug.LogError("No active Build Profile found.");
                return;
            }

            // Get current scenes in the profile
            var sceneList = activeProfile.scenes.ToList();

            bool modified = false;

            foreach (BackdropScene scene in backdropScenes)
            {
                if (scene == null)
                    continue;

                string scenePath = scene.GetScenePath();

                if (string.IsNullOrEmpty(scenePath))
                    continue;

                // Check if already present
                if (!sceneList.Any(s => s.path == scenePath))
                {
                    sceneList.Add(new EditorBuildSettingsScene
                    {
                        path = scenePath,
                        enabled = true
                    });

                    modified = true;
                }
            }

            if (modified)
            {
                activeProfile.scenes = sceneList.ToArray();
                EditorUtility.SetDirty(activeProfile);
                AssetDatabase.SaveAssets();

                Debug.Log("Build Profile scene list updated.");
            }
            else
            {
                Debug.Log("Build Profile already contains all scenes.");
            }
        }
#endif
    }
}