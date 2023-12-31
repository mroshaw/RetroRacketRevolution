using UnityEngine;
using Sirenix.OdinInspector;
using DaftAppleGames.RetroRacketRevolution.Menus;

namespace DaftAppleGames.RetroRacketRevolution.Game
{
    public enum LevelSelect { Original, Custom, OgPlusCustom, CustomPlusOg }

    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data", order = 1)]
    public class GameData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("Controls")] public string playerOneControlScheme;
        [BoxGroup("Controls")] public int playerOneControlSchemeIndex;
        [BoxGroup("Controls")] public string playerTwoControlScheme;
        [BoxGroup("Controls")] public int playerTwoControlSchemeIndex;
        [BoxGroup("Options Selected")] public bool isTwoPlayer;
        [BoxGroup("Options Selected")] public DifficultyData difficulty;
        [BoxGroup("Options Selected")] public LevelSelect levelSelect;
    }
}
