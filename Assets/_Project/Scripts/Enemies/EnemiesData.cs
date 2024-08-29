using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Enemies
{
    /// <summary>
    /// Scriptable Object for storing current Boss Enemy instance definitions
    /// </summary>
    [CreateAssetMenu(fileName = "Enemies", menuName = "Enemies/Enemies", order = 1)]
    public class EnemiesData : ScriptableObject
    {
        [BoxGroup("Settings")] public List<EnemyData> EnemyList;
    }
}
