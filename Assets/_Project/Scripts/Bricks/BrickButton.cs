using System;
using DaftApplesGames.RetroRacketRevolution.Levels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Bricks
{
    public class BrickButton : MonoBehaviour
    {
        public int x;
        public int y;

        private TextMeshProUGUI labelText;

        /// <summary>
        /// Set up the UI components
        /// </summary>
         private void Start()
        {
            Button button = GetComponentInChildren<Button>(true);
            labelText = button.GetComponentInChildren<TextMeshProUGUI>();
            button.onClick.AddListener(ButtonClicked);
            string[] coords = labelText.text.Split(",");

            x = Int32.Parse(coords[0]);
            y = Int32.Parse(coords[1]);
        }

        /// <summary>
        /// Brick is clicekd
        /// </summary>
        public void ButtonClicked()
        {
            Image image = GetComponentInChildren<Image>(true);

            BrickData brickData = LevelEditorManager.Instance.UpdateBrick(x, y);
            if (brickData != null)
            {
                labelText.text = brickData.Label;
                image.color = brickData.BrickColor;
            }
        }
    }
}
