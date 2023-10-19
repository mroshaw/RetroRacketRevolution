using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Levels
{
    /// <summary>
    /// Internal class describing a brick in the level
    /// </summary>
    [System.Serializable]
    public class BrickData
    {
        // Public properties
        public int RowNumber;
        public int ColumnNumber;
        public bool IsEmptySlot = false;
        public BrickType BrickType;
        public Color BrickColor;
        public BonusType BrickBonus;
        public string Label => GetBrickLabel();
        public Sprite bonusSprite;

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

        /// <summary>
        /// Get the label to use in the Level Editor
        /// </summary>
        /// <returns></returns>
        private string GetBrickLabel()
        {
            return ($"{RowNumber},{ColumnNumber},{GetBrickIsEmptyChar()},\n{GetBrickTypeLabel()},{GetBrickBonusLabel()}");
        }

        /// <summary>
        /// Get Brick Type char
        /// </summary>
        /// <returns></returns>
        private string GetBrickTypeLabel()
        {
            string brickTypeChar = "";
            switch (BrickType)
            {
                case BrickType.Normal:
                    brickTypeChar = "N";
                    break;
                case BrickType.DoubleStrong:
                    brickTypeChar = "D";
                    break;
                case BrickType.TripleStrong:
                    brickTypeChar = "T";
                    break;
                case BrickType.Invincible:
                    brickTypeChar = "I";
                    break;
                case BrickType.DisruptorIn:
                    brickTypeChar = "D1";
                    break;
                case BrickType.DisruptorOut:
                    brickTypeChar = "D2";
                    break;
                case BrickType.DisruptorBoth:
                    brickTypeChar = "D3";
                    break;
            }
            return brickTypeChar;
        }

        /// <summary>
        /// Get the Bonus label char
        /// </summary>
        /// <returns></returns>
        private string GetBrickBonusLabel()
        {
            string brickBonusChar = "";
            switch (BrickBonus)
            {
                case BonusType.None:
                    brickBonusChar = "NA";
                    break;
                case BonusType.Laser:
                    brickBonusChar = "LA";
                    break;
                case BonusType.GrowBat:
                    brickBonusChar = "BB";
                    break;
                case BonusType.BigScore:
                    brickBonusChar = "BS";
                    break;
                case BonusType.SmallScore:
                    brickBonusChar = "SS";
                    break;
                case BonusType.ExtraLife:
                    brickBonusChar = "EL";
                    break;
                case BonusType.FinishLevel:
                    brickBonusChar = "FL";
                    break;
                case BonusType.MegaBall:
                    brickBonusChar = "MB";
                    break;
                case BonusType.MultiBall:
                    brickBonusChar = "TB";
                    break;
                case BonusType.Random:
                    brickBonusChar = "RA";
                    break;
                case BonusType.SlowBall:
                    brickBonusChar = "SB";
                    break;
                case BonusType.ShrinkBat:
                    brickBonusChar = "LB";
                    break;
                case BonusType.Catcher:
                    brickBonusChar = "CA";
                    break;

            }
            return brickBonusChar;
        }

        /// <summary>
        /// Get the IsEmpty label char
        /// </summary>
        /// <returns></returns>
        private string GetBrickIsEmptyChar()
        {
            return IsEmptySlot ? "X" : "*";
        }
    }
}
