using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class HighScores
    {
        public HighScore[] HighScoreArray = new HighScore[5];

        private const string HighScoreNameKey = "HighScoreName";
        private const string HighScoreValueKey = "HighScoreValue";

        public HighScores()
        {
            for (int entry = 0; entry < HighScoreArray.Length; entry++)
            {
                HighScore newHighScore = new HighScore();
                HighScoreArray[entry] = newHighScore;
            }
            LoadHighScores();
        }

        public class HighScore : IComparable<HighScore>
        {
            public string PlayerName;
            public int Score;

            public HighScore()
            {
                PlayerName = "";
                Score = 0;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="playerName"></param>
            /// <param name="score"></param>
            public HighScore(string playerName, int score)
            {
                PlayerName = playerName;
                Score = score;
            }

            /// <summary>
            /// Sorting is by score
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public int CompareTo(HighScore other)
            {
                return -Score.CompareTo(other.Score);
            }
        }

        /// <summary>
        /// Submit a score
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool SubmitHighScore(string playerName, int score)
        {
            Array.Sort(HighScoreArray);
            int position = 0;
            foreach (HighScore highScore in HighScoreArray)
            {
                if (score > highScore.Score)
                {
                    // We have a new High Score
                    List<HighScore> highScoreList = HighScoreArray.ToList();
                    HighScore newHighScore = new HighScore(playerName, score);
                    highScoreList.Insert(position, newHighScore);
                    highScoreList.Remove(highScoreList.LastOrDefault());
                    HighScoreArray = highScoreList.ToArray();
                    Array.Sort(HighScoreArray);
                    SaveHighScores();
                    return true;
                }
                position++;
            }
            return false;
        }

        /// <summary>
        /// Gets the current high score
        /// </summary>
        /// <returns></returns>
        public int GetCurrentHighScore()
        {
            return HighScoreArray[0].Score;
        }

        /// <summary>
        /// Determines if submitted high score is a high score
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public bool IsHighScore(int score)
        {
            foreach (HighScore highScore in HighScoreArray)
            {
                if (score > highScore.Score)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// SaveHighScores to PlayerPrefs
        /// </summary>
        public void SaveHighScores()
        {
            int count = 0;
            foreach (HighScore highScore in HighScoreArray)
            {
                PlayerPrefs.SetString(HighScoreNameKey+count.ToString(), highScore.PlayerName);
                PlayerPrefs.SetInt(HighScoreValueKey + count.ToString(), highScore.Score);
                count++;
            }
        }

        /// <summary>
        /// Load High Scores from PlayerPrefs
        /// </summary>
        public void LoadHighScores()
        {
            int count = 0;
            foreach (HighScore highScore in HighScoreArray)
            {
                highScore.PlayerName = PlayerPrefs.GetString(HighScoreNameKey + count.ToString(), "AAA");
                highScore.Score = PlayerPrefs.GetInt(HighScoreValueKey + count.ToString(), (count + 1) * 1000);
                count++;
            }
            Array.Sort(HighScoreArray);
        }
    }
}
