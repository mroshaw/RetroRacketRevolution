using System;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DaftAppleGames.RetroRacketRevolution.LevelEditor
{
    public class BrickButton : MonoBehaviour
    {
        [BoxGroup("Position")] public int columnNumber;
        [BoxGroup("Position")] public int rowNumber;

        [BoxGroup("UI")] public Image buttonImage;
        [BoxGroup("UI")] public Image typeImage;
        [BoxGroup("UI")] public Image bonusImage;

        [BoxGroup("Data")] public BonusData bonusData;
        [BoxGroup("Data")] public BrickTypeData brickTypeData;

        [FoldoutGroup("Events")] public UnityEvent<int, int> brickButtonClickedEvent;

        public BrickData BrickData { get; set; }

        private Button _button;
        private Sprite _noBonusSprite;
        private Sprite _noBrickSprite;

        private TextMeshProUGUI _labelText;

        private void Awake()
        {
            _button = GetComponentInChildren<Button>(true);
            _labelText = _button.GetComponentInChildren<TextMeshProUGUI>(true);
            _noBonusSprite = bonusData.GetBonusByType(BonusType.None).levelEditorSprite;
            _noBrickSprite = brickTypeData.NoBrickSprite;
            string[] coords = _labelText.text.Split(",");
            rowNumber = Int32.Parse(coords[0]);
            columnNumber = Int32.Parse(coords[1]);

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
            brickButtonClickedEvent.Invoke(columnNumber, rowNumber);
        }

        /// <summary>
        /// Clears the button
        /// </summary>
        public void Clear()
        {
            buttonImage.color = Color.white;
            typeImage.sprite = _noBrickSprite;
            bonusImage.sprite = _noBonusSprite;
        }

        /// <summary>
        /// Update the button
        /// </summary>
        public void UpdateBrick(BrickData brickData)
        {
            // Is the update for me?
            if (brickData.rowNumber != rowNumber || brickData.columnNumber != columnNumber)
            {
                return;
            }

            BrickData = brickData;

            Color targetColor;
            _labelText.text = GetBrickLabel();
            if (BrickData.isEmptySlot)
            {
                targetColor = Color.white;
            }
            else if (BrickData.IsDisruptor())
            {
                targetColor = Color.grey;
            }
            else
            {
                targetColor = BrickData.brickColor;
            }

            Color newColor = targetColor;
            newColor.a = 0.99f;
            buttonImage.color = newColor;

            BonusData.BonusDef bonusDef = bonusData.GetBonusByType(BrickData.brickBonus);

            // Determine bonus info
            if (bonusDef == null)
            {
                Debug.LogError($"Could not find Bonus Def for type: {BrickData.brickBonus}!");
                return;
            }

            if (bonusDef.levelEditorSprite)
            {
                bonusImage.sprite = bonusData.GetBonusByType(BrickData.brickBonus).levelEditorSprite;
            }
            else
            {
                Debug.LogWarning($"Bonus Def entry for type {BrickData.brickBonus} does not contain a sprite image!");
            }

            // Determine brick info
            var brickDef = brickTypeData.GetBrickByType(BrickData.brickType);

            if (brickDef == null)
            {
                Debug.LogError($"Could not find Brick Def for type: {BrickData.brickType}!");
                return;
            }

            if (!brickDef.BrickSprite)
            {
                Debug.LogWarning($"Bonus Type entry for type {BrickData.brickType} does not contain a sprite image!");
            }

            typeImage.sprite = BrickData.isEmptySlot
                ? brickTypeData.NoBrickSprite
                : brickDef.BrickSprite;
        }

        /// <summary>
        /// Get the label to use in the Level Editor
        /// </summary>
        /// <returns></returns>
        private string GetBrickLabel()
        {
            return ($"{rowNumber},{columnNumber}");
        }

        /// <summary>
        /// Get Brick Type char
        /// </summary>
        /// <returns></returns>
        private string GetBrickTypeLabel()
        {
            string brickTypeChar = "";
            switch (BrickData.brickType)
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
            switch (BrickData.brickBonus)
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
            return BrickData.isEmptySlot ? "X" : "*";
        }
    }
}