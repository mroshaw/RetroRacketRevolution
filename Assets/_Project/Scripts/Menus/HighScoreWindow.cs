using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class HighScoreWindow : WindowBase
    {
        [BoxGroup("UI Settings")] public GameObject highScoreContainer;
        [BoxGroup("UI Settings")] public GameObject templateGameObject;

        /// <summary>
        /// Update with the latest High Scores
        /// </summary>
        private void Start()
        {
            RefreshHighScores();
        }

        /// <summary>
        /// Refresh scores before showing
        /// </summary>
        public override void Show()
        {
            RefreshHighScores();
            base.Show();
        }

        private void RefreshHighScores()
        {

            HighScores highScores = GameController.Instance.HighScores;

            // Clear down high scores
            foreach (Transform childTransform in highScoreContainer.transform)
            {
                Destroy(childTransform.gameObject);
            }

            // Recreate them
            foreach (HighScores.HighScore highScore in highScores.HighScoreArray)
            {
                GameObject newEntry = Instantiate(templateGameObject);
                newEntry.transform.SetParent(highScoreContainer.transform);
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.localPosition = new Vector3(0, 0, 0);
                newEntry.GetComponent<NameValueTemplate>().SetEntryText(highScore.PlayerName, highScore.Score.ToString());
                newEntry.SetActive(true);
            }
        }

        [Serializable]
        public class EntryTemplate
        {
            public GameObject templateGameObject;

        }
    }
}
