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
        public int rowNumber;
        public int columnNumber;
        public bool isEmptySlot;
        public BrickType brickType;
        public Color brickColor;
        public BonusType brickBonus;

        public bool HasIsEmptySlotChanged { get; set; }
        public bool HasBrickTypeChanged { get; set; }
        public bool HasBrickColorChanged { get; set; }
        public bool HasBrickBonusChanged { get; set; }

        public BrickData()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BrickData(BrickType brickType, Color color, BonusType brickBonus,
            bool isEmptySlot, int rowNumber, int columnNumber)
        {
            this.brickType = brickType;
            brickColor = color;
            this.brickBonus = brickBonus;
            this.isEmptySlot = isEmptySlot;
            this.rowNumber = rowNumber;
            this.columnNumber = columnNumber;
        }

        /// <summary>
        /// Returns true if the brick data instance is part of a Disruptor
        /// </summary>
        /// <returns></returns>
        public bool IsDisruptor()
        {
            return brickType == BrickType.DisruptorIn || brickType == BrickType.DisruptorOut ||
                   brickType == BrickType.DisruptorBoth;
        }
    }
}