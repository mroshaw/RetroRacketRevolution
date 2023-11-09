using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    /// <summary>
    /// Scriptable Object: TODO Purpose and Summary
    /// </summary>
    [CreateAssetMenu(fileName = "BrickTypeData", menuName = "Bricks/Brick Type Data", order = 1)]
    public class BrickTypeData : ScriptableObject
    {
        // Public serializable properties
        [BoxGroup("Brick Data")] public Sprite NoBrickSprite;
        [BoxGroup("Brick Data")] public List<BrickDef> BrickTypes;
        
        [Serializable]
        public class BrickDef
        {
            public BrickType Type;
            public Sprite BrickSprite;
            public Sprite BrickSpawnSprite;
            public string FriendlyName;
            public string Description;
        }

        /// <summary>
        /// Gets the BonusDef by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public BrickDef GetBrickByType(BrickType type)
        {
            foreach (BrickDef def in BrickTypes)
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
