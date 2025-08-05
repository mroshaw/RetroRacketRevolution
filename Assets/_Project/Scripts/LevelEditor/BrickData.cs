using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    /// <summary>
    /// Internal class describing a brick in the level
    /// </summary>
    [System.Serializable] public class BrickData
    {
        // Public properties
        public int RowNumber;
        public int ColumnNumber;
        public bool IsEmptySlot;
        public BrickType BrickType;
        public Color BrickColor;
        public BonusType BrickBonus;

        public bool HasIsEmptySlotChanged;
        public bool HasBrickTypeChanged;
        public bool HasBrickColorChanged;
        public bool HasBrickBonusChanged;

        public BrickData()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="brickType"></param>
        /// <param name="color"></param>
        /// <param name="brickBonus"></param>
        /// <param name="isEmptySlot"></param>
        /// <param name="rowNumber"></param>
        /// <param name="columnNumber"></param>
        public BrickData(BrickType brickType, Color color, BonusType brickBonus,
            bool isEmptySlot, int rowNumber, int columnNumber)
        {
            BrickType = brickType;
            BrickColor = color;
            BrickBonus = brickBonus;
            IsEmptySlot = isEmptySlot;
            RowNumber = rowNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        /// Returns true if the brick data instance is part of a Disruptor
        /// </summary>
        /// <returns></returns>
        public bool IsDisruptor()
        {
            return BrickType == BrickType.DisruptorIn || BrickType == BrickType.DisruptorOut ||
                   BrickType == BrickType.DisruptorBoth;
        }
    }
}