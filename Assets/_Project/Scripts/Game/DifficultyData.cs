using DaftApplesGames.RetroRacketRevolution.Levels;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Game
{
    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultyData", menuName = "Game/Difficulty Data", order = 1)]
    public class DifficultyData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public string difficultyName;
        [BoxGroup("Levels")] public LevelDataExt[] levels;
        [BoxGroup("Ball")] public float defaultBallSpeed;
        [BoxGroup("Ball")] public float ballSpeedUpAfterDuration;
        [BoxGroup("Ball")] public float ballSpeedMultiplier;
        [BoxGroup("Lives")] public int startingLives;
        [BoxGroup("Bat")] public float defaultBatLength;
    }
}
