using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace DaftAppleGames.Editor.ProjectTools
{
    public class BuildToolsEditorWindow : OdinEditorWindow
    {
        private const string BuildConfig = "Assets/_Project/RRRBuildSettings.asset";

        // Display Editor Window
        [MenuItem("Daft Apple Games/Build tool")]
        public static void ShowWindow()
        {
            GetWindow(typeof(BuildToolsEditorWindow));
        }

        [BoxGroup("Latest Build")] [SerializeField] [ReadOnly] private BuildTarget latestBuildTarget;
        [BoxGroup("Latest Build")] [SerializeField] [ReadOnly] private string latestBuildDateTime;
        [BoxGroup("Latest Build")] [SerializeField] [ReadOnly] private string latestBuildResult;
        [BoxGroup("Latest Build")] [SerializeField] [ReadOnly] private string latestBuildVersion;

        [BoxGroup("Platform Build")] [SerializeField] [ReadOnly] private BuildStatus winBuildStatus;
        [BoxGroup("Platform Build")] [SerializeField] [ReadOnly] private BuildStatus linuxBuildStatus;
        [BoxGroup("Platform Build")] [SerializeField] [ReadOnly] private BuildStatus androidBuildStatus;

        private BuildSettings _buildSettings;

        /// <summary>
        /// Initialise the build status when window is enabled
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
            LoadBuildSettings();
        }

        [BoxGroup("Build Control")] [Button("Build All", ButtonSizes.Large), GUIColor(0, 1, 0)]
        private void BuildAll()
        {
            BuildWinGame();
            BuildAndroidGame();
            BuildLinuxGame();
        }

        [BoxGroup("Build Control")] [Button("Build Windows Game")]
        private void BuildWinGame()
        {
            BuildPlayer(_buildSettings.winBuildTargetSettings, _buildSettings.winBuildStatus);
            // CopyLevels(_buildSettings.winBuildTargetSettings);
        }

        [BoxGroup("Build Control")] [Button("Build Linux Game")]
        private void BuildLinuxGame()
        {
            BuildPlayer(_buildSettings.linuxBuildTargetSettings, _buildSettings.linuxBuildStatus);
            // CopyLevels(_buildSettings.linuxBuildTargetSettings);
        }

        [BoxGroup("Build Control")] [Button("Build Android Game")]
        private void BuildAndroidGame()
        {
            BuildPlayer(_buildSettings.androidBuildTargetSettings, _buildSettings.androidBuildStatus);
            // CopyLevels(_buildSettings.androidBuildTargetSettings);
        }


        [BoxGroup("Deploy Control")] [Button("Deploy Windows to Itch", ButtonSizes.Large), GUIColor(1, 0, 0)]
        private void DeployWindowsToItch()
        {
            Thread newThead = DeployToItchInThread(_buildSettings.winBuildTargetSettings, _buildSettings.winBuildStatus);
        }


        [BoxGroup("Deploy Control")] [Button("Deploy Linux to Itch", ButtonSizes.Large), GUIColor(1, 0, 0)]
        private void DeployLinuxToItch()
        {
            Thread newThead = DeployToItchInThread(_buildSettings.linuxBuildTargetSettings, _buildSettings.linuxBuildStatus);
        }

        [BoxGroup("Deploy Control")] [Button("Deploy Linux to Itch", ButtonSizes.Large), GUIColor(1, 0, 0)]
        private void DeployAndroidToItch()
        {
            Thread newThead = DeployToItchInThread(_buildSettings.androidBuildTargetSettings, _buildSettings.androidBuildStatus);
        }

        /// <summary>
        /// Run the built game executable, to test prior to deployment to itch.io
        /// </summary>
        [BoxGroup("Run Control")] [Button("Run Windows Game"), GUIColor(0, 1, 0)]
        private void RunGame()
        {            Thread newThread = new Thread(RunGameInThread);
            newThread.Start();

        }

        [BoxGroup("Other Control")] [Button("Update Levels")]
        private void UpdateLevels()
        {
            // CopyLevels(_buildSettings.winBuildTargetSettings);
            // CopyLevels(_buildSettings.linuxBuildTargetSettings);
            // CopyLevels(_buildSettings.androidBuildTargetSettings);
        }

        [BoxGroup("Other Control")] [Button("Refresh Build Settings")]
        private void RefreshBuildSettings()
        {
            LoadBuildSettings();
        }


        /// <summary>
        /// Loads the latest build status settings
        /// </summary>
        private void LoadBuildSettings()
        {
            _buildSettings = (BuildSettings)AssetDatabase.LoadAssetAtPath(BuildConfig,
                typeof(BuildSettings));

            RefreshBuildStatus();
        }

        /// <summary>
        /// Refresh the window properties
        /// </summary>
        private void RefreshBuildStatus()
        {
            winBuildStatus = _buildSettings.winBuildStatus.CopyBuildStatus();
            linuxBuildStatus = _buildSettings.linuxBuildStatus.CopyBuildStatus();
            androidBuildStatus = _buildSettings.androidBuildStatus.CopyBuildStatus();

            latestBuildTarget = _buildSettings.latestBuildTarget;
            latestBuildDateTime = _buildSettings.latestBuildDateTime;
            latestBuildVersion = _buildSettings.latestBuildVersion;
            latestBuildResult = _buildSettings.latestBuildResult;
        }

        /// <summary>
        /// Runs the built game exe
        /// </summary>
        private void RunGameInThread()
        {
            Process process = new Process();

            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = _buildSettings.winBuildTargetSettings.fileName;
            int exitCode = -1;
            string output = null;

            try {
                process.Start();

                // do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // process.WaitForExit();

                // read the output stream first and then wait.
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e) {
                Debug.LogError("Run error" + e.ToString()); // or throw new Exception
            }
            finally {
                exitCode = process.ExitCode;

                process.Dispose();
                process = null;
            }
        }

        private Thread DeployToItchInThread(BuildTargetSettings buildTargetSettings, BuildStatus buildStatus)
        {
            Thread newThread = new Thread(() => DeployToItchInProcess(buildTargetSettings, buildStatus));
            newThread.Start();
            return newThread;
        }

        /// <summary>
        /// Deploy the build to itch.io
        /// </summary>
        private void DeployToItchInProcess(BuildTargetSettings buildTargetSettings, BuildStatus buildStatus)
        {
            Debug.Log("Starting deployment process. Please wait...");
            Process process = new Process();

            // Redirect the output stream of the child process.
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = buildTargetSettings.itchDeployBatchFullPath;
            process.StartInfo.Arguments = $"{buildTargetSettings.itchDeployAppName}  {buildTargetSettings.itchDeployAppStage}";
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(buildTargetSettings.itchDeployBatchFullPath) ?? string.Empty;

            int exitCode = -1;
            string output = null;

            try {
                process.Start();

                // do not wait for the child process to exit before
                // reading to the end of its redirected stream.
                // process.WaitForExit();

                // read the output stream first and then wait.
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e) {
                Debug.LogError("Run error" + e.ToString()); // or throw new Exception
            }
            finally {
                exitCode = process.ExitCode;

                process.Dispose();
                process = null;
            }

            Debug.Log($"Process completed with code: {exitCode}");
            Debug.Log($"Process output: {output}");

            buildStatus.lastDeployLog = output;

            if (exitCode == 0)
            {
                buildStatus.lastDeployState = DeployState.Success;
                buildStatus.lastSuccessfulDeploy.SetNow();

            }
            else
            {
                buildStatus.lastDeployState = DeployState.Failed;
            }

            RefreshBuildStatus();
        }

        /// <summary>
        /// Saves all open scenes
        /// </summary>
        private void SaveAllScenes()
        {
            int numScenes = EditorSceneManager.sceneCount;
            for (int currSceneIndex = 0; currSceneIndex < numScenes; currSceneIndex++)
            {
                EditorSceneManager.SaveScene(SceneManager.GetSceneAt(currSceneIndex));
            }
        }

        /// <summary>
        /// Call the aSync build player co-coroutine
        /// </summary>
        private void BuildPlayer(BuildTargetSettings buildTargetSettings, BuildStatus buildStatus)
        {
            EditorCoroutineUtility.StartCoroutine(BuildPlayerAsync(buildTargetSettings, buildStatus), this);
        }

        /// <summary>
        /// Async build player method, so we can monitor progress and update status
        /// </summary>
        /// <returns></returns>
        private IEnumerator BuildPlayerAsync(BuildTargetSettings buildTargetSettings, BuildStatus buildStatus)
        {
            PlayerSettings.bundleVersion = buildStatus.buildVersion.ToString();

            buildStatus.lastBuildAttempt.SetNow();

            BuildReport buildReport = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, buildTargetSettings.FullPath, buildTargetSettings.buildTarget, buildTargetSettings.buildOptions);
            while (BuildPipeline.isBuildingPlayer)
            {
                yield return null;
            }

            // Determine status of build and update the scriptable object instance and UI
            if (buildReport.summary.result == BuildResult.Succeeded)
            {
                // Set the build state and update the version number
                buildStatus.lastSuccessfulBuild.SetNow();
                _buildSettings.latestBuildVersion = buildStatus.buildVersion.ToString();
                buildStatus.buildVersion.IncrementVersion(_buildSettings.versionIncrementType);
                if (buildReport.summary is { totalErrors: 0, totalWarnings: 0 })
                {
                    buildStatus.lastBuildState = BuildState.Success;
                }
                else if (buildReport.summary.totalErrors > 0)
                {
                    buildStatus.lastBuildState = BuildState.SuccessWithErrors;
                }
                else if (buildReport.summary.totalWarnings > 0)
                {
                    buildStatus.lastBuildState = BuildState.SuccessWithWarnings;
                }
            }
            else
            {
                buildStatus.lastBuildState = BuildState.Failed;
            }

            string logEntries = "";
            foreach (BuildStepMessage stepMessage in buildReport.steps[0].messages)
            {
                logEntries += $"{stepMessage.content}\n";
            }

            buildStatus.lastBuildLog = logEntries.Trim();
            _buildSettings.latestBuildResult = buildStatus.lastBuildState.ToString();
            _buildSettings.latestBuildTarget = buildTargetSettings.buildTarget;
            _buildSettings.latestBuildDateTime = buildStatus.lastBuildAttempt.ToString();

            RefreshBuildStatus();
        }

        /// <summary>
        /// Sets the active scene
        /// </summary>
        /// <param name="sceneName"></param>
        private void SetActiveScene(string sceneName)
        {
            int numScenes = EditorSceneManager.sceneCount;
            Scene currScene;
            for (int currSceneIndex = 0; currSceneIndex < numScenes; currSceneIndex++)
            {
                currScene = SceneManager.GetSceneAt(currSceneIndex);
                if (currScene.name == sceneName)
                {
                    EditorSceneManager.SetActiveScene(currScene);
                }
            }
        }

        /// <summary>
        /// Copy levels to target
        /// </summary>
        private void CopyLevels(BuildTargetSettings buildTargetSettings)
        {
            // Copy level files, if not mobile platform
            if (buildTargetSettings.buildTarget == BuildTarget.StandaloneWindows64 || buildTargetSettings.buildTarget == BuildTarget.StandaloneLinux64)
            {
                string levelPath = buildTargetSettings.gameFolder;

                string levelDataPath = Path.Join(levelPath, "LevelData");
                string customLevelDataPath = Path.Join(levelPath, "CustomLevelData");

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

        /// <summary>
        /// Output a high level build summary
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="result"></param>
        /// <param name="totalErrors"></param>
        /// <param name="totalWarnings"></param>
        private static void BuildReportSummary(string platform, BuildResult result, int totalErrors, int totalWarnings)
        {
            Debug.Log($"Build summary for: {platform}:");
            Debug.Log($"Result: {result}");
            Debug.Log($"Total warnings: {totalWarnings}");
            Debug.Log($"Total Errors: {totalErrors}");

            if (totalErrors > 0)
            {
                Debug.LogError($"Error, build for platform {platform} failed with errors!");
            }

            if (totalWarnings > 0)
            {
                Debug.LogWarning($"Warning, build for platform {platform} failed with warnings!");
            }
        }

    }
}