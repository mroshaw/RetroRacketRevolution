using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class HudManager : MonoBehaviour
    {
        [BoxGroup("Lives")] [SerializeField] private GameObject livesPrefab;
        [BoxGroup("Lives")] [SerializeField] private GameObject livesContainer;
        [BoxGroup("Score")] [SerializeField] private TextMeshProUGUI player1ScoreText;
        [BoxGroup("Score")] [SerializeField] private TextMeshProUGUI highScoreText;
        [BoxGroup("Score")] [SerializeField] private TextMeshProUGUI player2ScoreText;

        private List<GameObject> _lives = new List<GameObject>();

        /// <summary>
        /// Update the lives UI
        /// </summary>
        public void NumLivesChanged(int numLives)
        {
            int currLives = _lives.Count;

            if (numLives == currLives)
            {
                return;
            }

            // Add lives
            if (numLives > currLives)
            {
                for (int currIndex = 0; currIndex < (numLives - currLives); currIndex++)
                {
                    GameObject newLife = Instantiate(livesPrefab);
                    newLife.transform.SetParent(livesContainer.transform, true);
                    newLife.transform.localScale = new Vector3(1, 1, 1);
                    _lives.Add(newLife);
                }
            }

            // Remove lives
            if (numLives < _lives.Count && numLives >= 0)
            {
                for (int currIndex = 0; currIndex < (currLives - numLives); currIndex++)
                {
                    GameObject lifeToRemove = _lives.Last();
                    _lives.Remove(lifeToRemove);
                    Destroy(lifeToRemove);
                }
            }
        }

        /// <summary>
        /// Handle Player 1 Score update
        /// </summary>
        public void Player1ScoreChanged(int score)
        {
            player1ScoreText.text = score.ToString();
        }

        /// <summary>
        /// Handle Player 2 Score update
        /// </summary>
        public void Player2ScoreChanged(int score)
        {
            player2ScoreText.text = score.ToString();
        }

        /// <summary>
        /// High score has changed
        /// </summary>
        public void HighScoreChanges(int score)
        {
            highScoreText.text = $"HI: {score.ToString()}";
        }
    }
}