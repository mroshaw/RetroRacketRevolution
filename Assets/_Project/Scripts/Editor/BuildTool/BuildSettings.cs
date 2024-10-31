using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DaftAppleGames.Editor.BuildTool
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Daft Apple Games/Project/Build settings", order = 1)]
    public class BuildSettings : ScriptableObject
    {
        [BoxGroup("Build Scenes")] public SceneAsset[] sceneAssets;
        [BoxGroup("Build Scenes")] public SceneAsset emptySceneAsset;
        [BoxGroup("Build Settings")] public VersionIncrementType versionIncrementType;

        [BoxGroup("Windows Settings")] public BuildTargetSettings winBuildTargetSettings;
        [BoxGroup("Windows Settings")] public BuildStatus winBuildStatus;

        [BoxGroup("Linux Settings")] public BuildTargetSettings linuxBuildTargetSettings;
        [BoxGroup("Linux Settings")] public BuildStatus linuxBuildStatus;

        [BoxGroup("Android Settings")] public BuildTargetSettings androidBuildTargetSettings;
        [BoxGroup("Android Settings")] public BuildStatus androidBuildStatus;

        [BoxGroup("Latest Build")][SerializeField] public BuildTarget latestBuildTarget;
        [BoxGroup("Latest Build")][SerializeField] public string latestBuildDateTime;
        [BoxGroup("Latest Build")][SerializeField] public string latestBuildResult;
        [BoxGroup("Latest Build")][SerializeField] public string latestBuildVersion;

        public List<string> GetAllScenePaths()
        {
            List<string> scenePaths = new();

            foreach (SceneAsset currScene in sceneAssets)
            {
                scenePaths.Add(AssetDatabase.GetAssetPath(currScene));
            }

            return scenePaths;
        }
    }
}