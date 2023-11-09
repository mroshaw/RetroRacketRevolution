using System;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using DaftAppleGames.RetroRacketRevolution.Levels;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class BrickButton : MonoBehaviour
    {
        [BoxGroup("Position")] public int ColumnNumber;
        [BoxGroup("Position")] public int RowNumber;

        [BoxGroup("UI")] public Image buttonImage;
        [BoxGroup("UI")] public Image typeImage;
        [BoxGroup("UI")] public Image bonusImage;

        [BoxGroup("Data")] public BonusData bonusData;
        [BoxGroup("Data")] public BrickTypeData brickTypeData;

        [FoldoutGroup("Events")] public UnityEvent<int, int> BrickButtonClickedEvent;

        public BrickData BrickData { get; set; }

        private Button _button;
        private TextMeshProUGUI _labelText;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>(true);
            _labelText = _button.GetComponentInChildren<TextMeshProUGUI>(true);

            string[] coords = _labelText.text.Split(",");
            RowNumber = Int32.Parse(coords[0]);
            ColumnNumber = Int32.Parse(coords[1]);

            _button.onClick.AddListener(ButtonClicked);
        }

        /// <summary>
        /// Set up the UI components
        /// </summary>
        private void Start()
        {
            _button.onClick.AddListener(ButtonClicked);
        }

        /// <summary>
        /// Brick is clicked
        /// </summary>
        public void ButtonClicked()
        {
            BrickButtonClickedEvent.Invoke(ColumnNumber, RowNumber);
        }

        /// <summary>
        /// Update the button
        /// </summary>
        public void UpdateBrick(BrickData brickData)
        {
            // Is the update for me?
            if (brickData.RowNumber != RowNumber || brickData.ColumnNumber != ColumnNumber)
            {
                return;
            }

            BrickData = brickData;

            Color targetColor;
            _labelText.text = GetBrickLabel();
            if (BrickData.IsEmptySlot)
            {
                targetColor = Color.white;
            }
            else if (BrickData.IsDisruptor())
            {
                targetColor = Color.grey;
            }
            else
            {
                targetColor = BrickData.BrickColor;
            }
            Color newColor = targetColor;
            newColor.a = 0.99f;
            buttonImage.color = newColor;
            bonusImage.sprite = bonusData.GetBonusByType(BrickData.BrickBonus).SpawnSprite;
            typeImage.sprite = BrickData.IsEmptySlot ? brickTypeData.NoBrickSprite : brickTypeData.GetBrickByType(BrickData.BrickType).BrickSprite;
        }

        /// <summary>
        /// Get the label to use in the Level Editor
        /// </summary>
        /// <returns></returns>
        private string GetBrickLabel()
        {
            return ($"{RowNumber},{ColumnNumber}");
        }

        /// <summary>
        /// Get Brick Type char
        /// </summary>
        /// <returns></returns>
        private string GetBrickTypeLabel()
        {
            string brickTypeChar = "";
            switch (BrickData.BrickType)
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
            switch (BrickData.BrickBonus)
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
            return BrickData.IsEmptySlot ? "X" : "*";
        }
    }
}
