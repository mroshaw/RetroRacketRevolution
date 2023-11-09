using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public enum BonusType { None, MultiBall, Laser, SlowBall, MegaBall, ExtraLife, FinishLevel, Random, SmallScore, BigScore, ShrinkBat, GrowBat, Catcher }

    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "BonusData", menuName = "Bonuses/Bonus Data", order = 1)]
    public class BonusData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("Bonus Data")]
        public List<BonusDef> Bonuses;

        [Serializable]
        public class BonusDef
        {
            public BonusType Type;
            public GameObject SpawnPrefab;
            public Sprite SpawnSprite;
            public string FriendlyName;
            public string Description;
        }

        /// <summary>
        /// Gets the BonusDef by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BonusDef GetBonusByType(BonusType type)
        {
            foreach (BonusDef def in Bonuses)
            {
                if (def.Type == type)
                {
                    return def;
                }
            }
            return null;
        }
    }
}
