using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    [Serializable] public class BackdropScene
    {
        [TableColumnWidth(120, Resizable = true)] [SerializeField] private string sceneName;
        public string SceneName => sceneName;
#if UNITY_EDITOR
        [TableColumnWidth(180, Resizable = true)] [OnValueChanged("UpdateName")] [SerializeField]
        private SceneAsset sceneAsset;

        [TableColumnWidth(20, Resizable = true)]
        [Button("Update Name")]
        private void UpdateName()
        {
            sceneName = sceneAsset.name;
        }

        [Button("Load Now")]
        private void LoadNow()
        {
            BackdropSceneLoader loader = Object.FindFirstObjectByType<BackdropSceneLoader>();

            // Remove current backdrop scene, if one is loaded.
            loader.UnloadCurrentSceneInEditor();

            // Load the scene via the BackdropSceneLoader
            string assetPath = AssetDatabase.GetAssetPath(sceneAsset);
            loader.OpenSceneInEditor(assetPath, sceneName);
        }
#endif
#if UNITY_EDITOR
        public string GetScenePath()
        {
            return AssetDatabase.GetAssetPath(sceneAsset);
        }
#endif
    }
}