using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DaftAppleGames.Editor
{
    public enum BuildPlatform { Windows, Linux, Android }

    public class BuildTool : OdinEditorWindow
    {
        public static string WinGameFolder = @"C:\Games\Retro Racket Revolution";
        public static string LinuxGameFolder = @"C:\Games\Retro Racket Revolution Linux";
        public static string AndroidGameFolder = @"C:\Games\Retro Racket Revolution Android";

        public static string WinFileName = @"Retro Racket Revolution.exe";
        public static string LinuxFileName = @"RetroRacketRevolution.x86_64";
        public static string AndroidFileName = @"RetroRacketRevolution.apk";

        [MenuItem("Build Tools/Build")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildTool));
        }
        
        [Button("BuildAll", ButtonSizes.Large), GUIColor(0, 1, 0)]
        private void BuildAll()
        {
            BuildAndroidGame();
            BuildLinuxGame();
            BuildWinGame();
        }
        
        [Button("Build Windows Game")]
        private void BuildWinGame()
        {
            BuildGame(BuildPlatform.Windows);
        }

        [Button("Build Linux Game")]
        private void BuildLinuxGame()
        {
            BuildGame(BuildPlatform.Windows);
        }

        [Button("Build Android Game")]
        private void BuildAndroidGame()
        {
            BuildGame(BuildPlatform.Android);
        }

        /// <summary>
        /// Build the game for the given platform
        /// </summary>
        /// <param name="buildPlatform"></param>
        private static void BuildGame(BuildPlatform buildPlatform)
        {
            // Scenes to build
            string[] scenesToBuild = new string[] { "Assets/_Project/Scenes/MainMenuScene.unity", "Assets/_Project/Scenes/GameScene.unity", "Assets/_Project/Scenes/LevelEditorScene.unity" };
            
            // Determine filename, build options
            string path = "";
            string fileName = "";
            BuildTarget buildTarget;

            switch (buildPlatform)
            {
                case BuildPlatform.Windows:
                    path = WinGameFolder;
                    fileName = WinFileName;
                    buildTarget = BuildTarget.StandaloneWindows64;
                    break;
                case BuildPlatform.Android:
                    path = AndroidGameFolder;
                    fileName = AndroidFileName;
                    buildTarget = BuildTarget.Android;
                    break;
                case BuildPlatform.Linux:
                    path = LinuxGameFolder;
                    fileName = LinuxFileName;
                    buildTarget = BuildTarget.StandaloneLinux64;
                    break;

                default:
                    buildTarget = BuildTarget.StandaloneWindows;
                    path = WinGameFolder;
                    fileName = WinFileName;
                    break;
            }

            Debug.Log($"Building: {buildPlatform} to {path + "//" + fileName}");

            // Set up options

            // Build player.
            BuildPipeline.BuildPlayer(scenesToBuild, path + "//" + fileName, buildTarget, BuildOptions.CompressWithLz4HC);

            // Copy level files, if not mobile platform
            if (buildPlatform == BuildPlatform.Windows || buildPlatform == BuildPlatform.Linux)
            {
                // Copy the level files
                string levelDataPath = $@"{path}//LevelData";
                string customLevelDataPath = $@"{path}//CustomLevelData";

                Debug.Log($"Copying level data to {levelDataPath}");

                if (!Directory.Exists(levelDataPath))
                {
                    Directory.CreateDirectory(levelDataPath);
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
