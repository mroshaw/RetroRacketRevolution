using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Enemies
{
    /// <summary>
    /// Scriptable Object representing properties of an enemy
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Enemies/Enemy Data", order = 1)]
    public class EnemyData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("General Settings")] public string enemyName;
        [BoxGroup("General Settings")] public GameObject enemyPrefab;
        [BoxGroup("General Settings")] public Sprite sprite;
    }
}
