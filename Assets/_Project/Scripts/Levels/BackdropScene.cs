using System;
using Sirenix.OdinInspector;
using UnityEngine;
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
        [Button("U")]
        [LabelText("U")]
        private void UpdateName()
        {
            sceneName = sceneAsset.name;
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