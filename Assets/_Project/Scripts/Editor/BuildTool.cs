using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.IO;
using DaftAppleGames.RetroRacketRevolution.Editor;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DaftAppleGames.Editor
{
    public class BuildTool : OdinEditorWindow
    {
        public static string WinGameFolder = @"E:\Dev\DAG\Itch Builds\Retro Racket Revolution\Retro Racket Revolution Windows";
        public static string LinuxGameFolder = @"E:\Dev\DAG\Itch Builds\Retro Racket Revolution\Retro Racket Revolution Linux";
        public static string AndroidGameFolder = @"E:\Dev\DAG\Itch Builds\Retro Racket Revolution\Retro Racket Revolution Android";

        public static string WinFileName = @"Retro Racket Revolution.exe";
        public static string LinuxFileName = @"RetroRacketRevolution.x86_64";
        public static string AndroidFileName = @"RetroRacketRevolution.apk";

        [MenuItem("Build Tools/Build Tool Window")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildTool));
        }

        [MenuItem("Build Tools/Build All")]
        [Button("BuildAll", ButtonSizes.Large), GUIColor(0, 1, 0)]
        private static void BuildAll()
        {
            VersionIncrementor.IncreaseBuild();
            BuildAndroidGame();
            BuildLinuxGame();
            BuildWinGame();
        }
        
        [Button("Build Windows Game")]
        private static void BuildWinGame()
        {
            BuildGame(BuildTarget.StandaloneWindows64);
            CopyLevels(BuildTarget.StandaloneWindows64);
        }

        [Button("Build Linux Game")]
        private static void BuildLinuxGame()
        {
            BuildGame(BuildTarget.StandaloneLinux64);
            CopyLevels(BuildTarget.StandaloneLinux64);
        }

        [Button("Build Android Game")]
        private static void BuildAndroidGame()
        {
            BuildGame(BuildTarget.Android);
            CopyLevels(BuildTarget.Android);
        }

        [MenuItem("Build Tools/Update Levels")]
        [Button("Update Levels")]
        private static void UpdateLevels()
        {
            CopyLevels(BuildTarget.Android);
            CopyLevels(BuildTarget.StandaloneWindows64);
            CopyLevels(BuildTarget.StandaloneLinux64);
        }

        /// <summary>
        /// Build the game for the given platform
        /// </summary>
        /// <param name="buildTarget"></param>
        private static void BuildGame(BuildTarget buildTarget)
        {
            // Scenes to build
            string[] scenesToBuild = new string[] { "Assets/_Project/Scenes/MainMenuScene.unity", "Assets/_Project/Scenes/GameScene.unity", "Assets/_Project/Scenes/LevelEditorScene.unity" };

            BuildOptions buildOptions = BuildOptions.CompressWithLz4HC;

            // Set up options
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    break;
                case BuildTarget.Android:
                    break;
                case BuildTarget.StandaloneLinux64:
                    break;
            }

            // Get build paths
            GetBuildPaths(buildTarget, out string path, out string fileName);

            Debug.Log($"Building: {buildTarget} to {path + "//" + fileName}");

            // Build player.
            BuildPipeline.BuildPlayer(scenesToBuild, path + "//" + fileName, buildTarget, buildOptions);
        }

        /// <summary>
        /// Get build path and filename
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <param name="outputPath"></param>
        /// <param name="outputFilename"></param>
        private static void GetBuildPaths(BuildTarget buildTarget, out string outputPath, out string outputFilename)
        {
            string path = "";
            string fileName = "";

            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    path = WinGameFolder;
                    fileName = WinFileName;
                    break;
                case BuildTarget.Android:
                    path = AndroidGameFolder;
                    fileName = AndroidFileName;
                    break;
                case BuildTarget.StandaloneLinux64:
                    path = LinuxGameFolder;
                    fileName = LinuxFileName;
                    break;
            }
            outputPath = path;
            outputFilename = fileName;
        }

        /// <summary>
        /// Copy levels to target
        /// </summary>
        /// <param name="buildTarget"></param>
        private static void CopyLevels(BuildTarget buildTarget)
        {
            // Copy level files, if not mobile platform
            if (buildTarget ==BuildTarget.StandaloneWindows64 || buildTarget == BuildTarget.StandaloneLinux64)
            {
                // Get build paths
                GetBuildPaths(buildTarget, out string path, out string fileName);

                string levelDataPath = $@"{path}//LevelData";
                string customLevelDataPath = $@"{path}//CustomLevelData";

                Debug.Log($"Copying level data to {levelDataPath}");

                // Create target directories, if it doesn't exist
                if (!Directory.Exists(levelDataPath))
                {
                    Debug.Log($"Creating directory: {levelDataPath}");
                    Directory.CreateDirectory(levelDataPath);
                }

                if (!Directory.Exists(customLevelDataPath))
                {
                    Debug.Log($"Creating directory: {customLevelDataPath}");
                    Directory.CreateDirectory(customLevelDataPath);
                }

                // Copy main game levels
                foreach (string currFile in Directory.GetFiles("E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\LevelData", "*.json"))
                {
                    string destFilePath = $"{levelDataPath}\\{Path.GetFileName(currFile)}";
                    Debug.Log($"Copying {currFile} level data to {destFilePath}");

                    FileUtil.ReplaceFile(currFile, destFilePath);
                }

                // Copy example custom levels
                foreach (string currFile in Directory.GetFiles("E:\\Dev\\DAG\\Retro Racket Revolution\\Assets\\_Project\\Resources\\CustomLevelData", "*.json"))
                {
                    string destFilePath = $"{customLevelDataPath}\\{Path.GetFileName(currFile)}";
                    Debug.Log($"Copying {currFile} level data to {destFilePath}");

                    FileUtil.ReplaceFile(currFile, destFilePath);
                }
            }
        }
    }
}

