using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public enum BonusType
    {
        None,
        MultiBall,
        Laser,
        SlowBall,
        MegaBall,
        ExtraLife,
        FinishLevel,
        Random,
        SmallScore,
        BigScore,
        ShrinkBat,
        GrowBat,
        Catcher
    }

    [CreateAssetMenu(fileName = "BonusData", menuName = "Bonuses/Bonus Data", order = 1)]
    public class BonusData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("Bonus Data")] public List<BonusDef> bonuses;

        [Serializable] public class BonusDef
        {
            [BoxGroup("Settings")] public BonusType type;
            [BoxGroup("Settings")] public GameObject spawnPrefab;
            [BoxGroup("Level Editor")] public Sprite levelEditorSprite;
            [BoxGroup("Level Editor")] public string friendlyName;
            [BoxGroup("Level Editor")] public string description;
        }

        /// <summary>
        /// Gets the BonusDef by type
        /// </summary>
        public BonusDef GetBonusByType(BonusType type)
        {
            foreach (BonusDef def in bonuses)
            {
                if (def.type == type)
                {
                    return def;
                }
            }

            return null;
        }
    }
}