using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace DaftAppleGames.Editor.BuildTool
{
    public class SceneLoaderWindow : OdinEditorWindow
    {
        [HideIf("@mainMenuSceneAsset != null")] [SerializeField] private SceneAsset mainMenuSceneAsset;
        [HideIf("@gameSceneAsset != null")] [SerializeField] private SceneAsset gameSceneAsset;
        [HideIf("@emptySceneAsset != null")] [SerializeField] private SceneAsset emptySceneAsset;
        [HideIf("@modelSceneAsset != null")] [SerializeField] private SceneAsset modelSceneAsset;

        // Display Editor Window
        [MenuItem("Daft Apple Games/Editor/Scene Loader")]
        public static void ShowWindow()
        {
            EditorWindow editorWindow = GetWindow(typeof(SceneLoaderWindow));
            editorWindow.titleContent = new GUIContent("Scene Loader");
            editorWindow.Show();
        }

        [BoxGroup("Game Scenes")]
        [Button("Main Menu", ButtonSizes.Medium), GUIColor(0, 1, 0)]
        private void LoadMainMenuScenes()
        {
            EditorSceneManager.SaveOpenScenes();
            OpenScene(mainMenuSceneAsset);
        }

        [BoxGroup("Game Scenes")]
        [Button("Game", ButtonSizes.Medium), GUIColor(0, 1, 0)]
        private void LoadGameScenes()
        {
            EditorSceneManager.SaveOpenScenes();
            OpenScene(gameSceneAsset);
        }

        [BoxGroup("Editor Scenes")]
        [Button("Model Scene", ButtonSizes.Medium)]
        private void LoadModelScene()
        {
            EditorSceneManager.SaveOpenScenes();
            OpenScene(modelSceneAsset);
        }


        [BoxGroup("Editor Scenes")]
        [Button("Empty Scene", ButtonSizes.Medium)]
        private void LoadEmptyScene()
        {
            EditorSceneManager.SaveOpenScenes();
            OpenScene(emptySceneAsset);
        }

        private void OpenScene(SceneAsset sceneAsset)
        {
            EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(sceneAsset), OpenSceneMode.Single);
        }
    }
}