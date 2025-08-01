using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Game
{
    /// <summary>
    /// Runtime game configuration settings
    /// </summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config", order = 1)]
    public class GameConfig : ScriptableObject
    {
        [BoxGroup("Scenes")] public string gameScene;
        [BoxGroup("Scenes")] public string menuScene;
        [BoxGroup("Scenes")] public string levelEditorScene;
        [BoxGroup("Scenes")] public string loaderScene;
        [BoxGroup("Scenes")] public string initScene;
    }
}
