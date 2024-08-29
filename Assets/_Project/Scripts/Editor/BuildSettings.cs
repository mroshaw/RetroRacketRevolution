using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace DaftAppleGames.Editor
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Daft Apple Games/Project/Build settings", order = 1)]
    public class BuildSettings : ScriptableObject
    {
        [BoxGroup("Build Settings")] public VersionIncrementType versionIncrementType;

        [BoxGroup("Windows Settings")] public BuildTargetSettings winBuildTargetSettings;
        [BoxGroup("Windows Settings")] public BuildStatus winBuildStatus;

        [BoxGroup("Linux Settings")] public BuildTargetSettings linuxBuildTargetSettings;
        [BoxGroup("Linux Settings")] public BuildStatus linuxBuildStatus;

        [BoxGroup("Android Settings")] public BuildTargetSettings androidBuildTargetSettings;
        [BoxGroup("Android Settings")] public BuildStatus androidBuildStatus;

        [BoxGroup("Latest Build")] [SerializeField] public BuildTarget latestBuildTarget;
        [BoxGroup("Latest Build")] [SerializeField] public string latestBuildDateTime;
        [BoxGroup("Latest Build")] [SerializeField] public string latestBuildResult;
        [BoxGroup("Latest Build")] [SerializeField] public string latestBuildVersion;
    }
}