using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace DaftAppleGames.Editor
{
    [InitializeOnLoad]
    class VersionIncrementor
    {
        public int callbackOrder => 0;

        private static string lastVersion;

        [MenuItem("Build Tools/Increase Both Versions &v")]
        public static void IncreaseBothVersions()
        {
            IncreaseBuild();
            IncreasePlatformVersion();
        }
        [MenuItem("Build Tools/Increase Current Build Version")]
        public static void IncreaseBuild()
        {
            IncrementVersion(new[] { 0, 0, 1 });
        }
        [MenuItem("Build Tools/Increase Minor Version")]
        public static void IncreaseMinor()
        {
            IncrementVersion(new[] { 0, 1, 0 });
        }
        [MenuItem("Build Tools/Increase Major Version")]
        public static void IncreaseMajor()
        {
            IncrementVersion(new[] { 1, 0, 0 });
        }
        [MenuItem("Build Tools/Increase Platform Version")]
        public static void IncreasePlatformVersion()
        {
            PlayerSettings.Android.bundleVersionCode += 1;
            PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
        }
        private static void IncrementVersion(int[] version)
        {
            var rawVer = version.Clone() as int[];
            string[] lines = PlayerSettings.bundleVersion.Split('.');
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                bool isNumber = int.TryParse(lines[i], out int numberValue);
                if (isNumber && version.Length - 1 >= i)
                    version[i] += numberValue;
            }

            // Clears the lowest versions by higher
            bool toZero = false;
            for (int i = 0; i < rawVer.Length; i++)
            {
                if (toZero)
                    version[i] = 0;
                else if (rawVer[i] == 1)
                    toZero = true;
            }

            lastVersion = PlayerSettings.bundleVersion;
            PlayerSettings.bundleVersion = $"{version[0]}.{version[1]}.{version[2]}";
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            int shouldIncrement = EditorUtility.DisplayDialogComplex(
                "Increment Version",
                $"Current: {PlayerSettings.bundleVersion}",
                "Patch (0.0.X)",
                "Major(X.0.0)",
                "Minor(0.X.0)"
            );

            switch (shouldIncrement)
            {
                case 0: IncreaseBuild(); break;
                case 1: IncreaseMajor(); break;
                case 2: IncreaseMinor(); break;
            }

            IncreasePlatformVersion();
            Application.logMessageReceived += OnBuildError;
        }
        private void OnBuildError(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Application.logMessageReceived -= OnBuildError;
                Debug.LogError($"Version rolled from {PlayerSettings.bundleVersion} to {lastVersion} due to build error");
                PlayerSettings.bundleVersion = lastVersion;
            }
        }
        public void OnPostprocessBuild(BuildReport report)
        {
            Application.logMessageReceived -= OnBuildError;
        }
    }
}