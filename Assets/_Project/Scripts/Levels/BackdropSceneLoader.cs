using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    public class BackdropSceneLoader : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] [InlineEditor] private LevelBackdropScenes levelBackdropScenes;

        [BoxGroup("Debug")] [SerializeField] private string currentlyLoadedSceneName;
        [BoxGroup("Debug")] [SerializeField] private bool isSceneLoaded = false;
#if UNITY_EDITOR
        [BoxGroup("Editor Debug")] [SerializeField] private Scene currentlyLoadedSceneInEditor;
        [BoxGroup("Editor Debug")] [SerializeField] private bool isSceneLoadedInEditor;
#endif

        /// <summary>
        /// Load the scene given the scene index
        /// </summary>
        public void SetScene(int sceneIndex)
        {
            string sceneName = levelBackdropScenes.GetScene(sceneIndex);
            SetScene(sceneName);
        }

        /// <summary>
        /// SLoad the scene given the scene name
        /// </summary>
        public void SetScene(string sceneName)
        {
            if (levelBackdropScenes == null)
            {
                Debug.LogError(
                    "BackdropSceneLoader SetScene called without an active BackdropSceneLoader component in the scene!");
                return;
            }

            StartCoroutine(LoadBackdropScene(sceneName));
        }

        /// <summary>
        /// Return a list of scene names, used by the level editor
        /// </summary>
        /// <returns></returns>
        public List<string> GetBackdropSceneNames()
        {
            if (!levelBackdropScenes)
            {
                return new List<string>();
            }

            return levelBackdropScenes.GetSceneNames();
        }

        /// <summary>
        /// Unloads the current backdrop scene
        /// </summary>
        public void UnloadCurrentScene()
        {
            // Stop any active coroutine
            StartCoroutine(UnloadCurrentSceneAsync());
        }
#if UNITY_EDITOR
        /// <summary>
        /// Open scene in editor mode
        /// </summary>
        public void OpenSceneInEditor(string scenePath, string sceneName)
        {
            currentlyLoadedSceneInEditor = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            isSceneLoadedInEditor = true;
        }

        /// <summary>
        /// Save and close the current backdrop scene in the editor
        /// </summary>
        public void UnloadCurrentSceneInEditor()
        {
            if (!isSceneLoadedInEditor)
            {
                return;
            }

            EditorSceneManager.SaveOpenScenes();
            EditorSceneManager.CloseScene(currentlyLoadedSceneInEditor, true);
            isSceneLoadedInEditor = false;
        }
#endif
        /// <summary>
        /// Loads the given backdrop scene then sets the scene properties
        /// </summary>
        private IEnumerator LoadBackdropScene(string sceneName)
        {
            // Unload the current scene
            yield return UnloadCurrentSceneAsync();

            // Load the new scene
            AsyncOperation sceneAsyncProcess = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (sceneAsyncProcess == null)
            {
                Debug.LogError($"Unable to load background scene: {sceneName}!!!");
                yield break;
            }

            // Wait for scene to load
            while (!sceneAsyncProcess.isDone)
            {
                yield return null;
            }

            // Once loaded, run the callback
            OnSceneLoaded(sceneName);
        }

        /// <summary>
        /// Unloads the current scene, if loaded, then resets the currently loaded scene properties
        /// </summary>
        private IEnumerator UnloadCurrentSceneAsync()
        {
            if (String.IsNullOrEmpty(currentlyLoadedSceneName) || !isSceneLoaded)
            {
                Debug.Log("No scene to unload.");
                yield break;
            }

            AsyncOperation sceneAsyncProcess = SceneManager.UnloadSceneAsync(currentlyLoadedSceneName);

            if (sceneAsyncProcess == null)
            {
                Debug.LogError($"Unable to unload background scene: {currentlyLoadedSceneName}!!!");
                yield break;
            }

            while (!sceneAsyncProcess.isDone)
            {
                yield return null;
            }

            // Once unloaded, run the callback
            OnCurrentSceneUnLoaded();
        }

        /// <summary>
        /// Called after a background scene is unloaded
        /// </summary>
        private void OnCurrentSceneUnLoaded()
        {
            currentlyLoadedSceneName = "";
            isSceneLoaded = false;
        }

        /// <summary>
        /// Called when a background scene is loaded
        /// </summary>
        private void OnSceneLoaded(string sceneName)
        {
            currentlyLoadedSceneName = sceneName;
            isSceneLoaded = true;
        }
    }
}