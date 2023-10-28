using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Effects;
using DaftAppleGames.RetroRacketRevolution.Game;
using DaftApplesGames.RetroRacketRevolution.Balls;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using DaftApplesGames.RetroRacketRevolution.Levels;
using DaftApplesGames.RetroRacketRevolution.Menus;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static DaftApplesGames.RetroRacketRevolution.Menus.HighScores;

namespace DaftApplesGames.RetroRacketRevolution.Game
{
    public enum AlertType {StartLevel, FinishLevel, GameOver, GameComplete }

    public class GameManager : MonoBehaviour
    {
        [BoxGroup("Managers")] public BallManager ballManager;
        [BoxGroup("Managers")] public BrickManager brickManager;
        [BoxGroup("Managers")] public LevelLoader levelLoader;
        [BoxGroup("Managers")] public AddOnManager addOnManager;
        [BoxGroup("Managers")] public PlayerManager playerManager;

        [BoxGroup("UI")] public GameObject infoPanel;
        [BoxGroup("UI")] public GameObject startLevel;
        [BoxGroup("UI")] public TextMeshProUGUI startLevelNameText;
        [BoxGroup("UI")] public GameObject finishLevel;
        [BoxGroup("UI")] public GameObject gameOver;
        [BoxGroup("UI")] public GameObject gameComplete;
        [BoxGroup("UI")] public CircleTextInput newHighScoreInput;
        [BoxGroup("UI")] public float gameOverPanelDuration = 5.0f;
        [BoxGroup("UI")] public float gameCompletePanelDuration = 5.0f;
        [BoxGroup("UI")] public float levelCompletePanelDuration = 3.0f;
        [BoxGroup("UI")] public float levelStartPanelDuration = 3.0f;

        [BoxGroup("Audio")] public AudioClip levelStartedClip;
        [BoxGroup("Audio")] public AudioClip levelCompleteClip;
        [BoxGroup("Audio")] public AudioClip gameOverClip;
        [BoxGroup("Audio")] public AudioClip gameCompletedClip;
        
        [BoxGroup("Object Tracking")] public Player player1;
        [BoxGroup("Object Tracking")] public Player player2;

        [FoldoutGroup("Events")] public UnityEvent<int> HighScoreChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent GameOverEvent;
        [FoldoutGroup("Events")] public UnityEvent GameCompleteEvent;
        [FoldoutGroup("Events")] public UnityEvent LevelCompleteEvent;
        [FoldoutGroup("Alert Events")] public UnityEvent BeforeAlertEvent;
        [FoldoutGroup("Alert Events")] public UnityEvent AfterAlertEvent;

        private HighScores highScores;

        private const string HighScore = "HighScore";

        private AudioSource _audioSource;

        /// <summary>
        /// Set up the Game Manager
        /// </summary>
        private void Awake()
        {

            _audioSource = GetComponent<AudioSource>();
            HideAlert();
        }

        /// <summary>
        /// Set up the UI
        /// </summary>
        private void Start()
        {
            // Set unlimited framerate
            Application.targetFrameRate = -1;
            highScores = new HighScores();
            highScores.LoadHighScores();
            GetHighScore();
        }

        /// <summary>
        /// Get the current high score
        /// </summary>
        private void GetHighScore()
        {
            int highScore = highScores.GetCurrentHighScore();
            HighScoreChangedEvent.Invoke(highScore);
        }

        /// <summary>
        /// Handle Level Complete
        /// </summary>
        public void LevelComplete()
        {
            // Debug.Log("Level Complete Called...");
            ballManager.DestroyAllBalls();
            _audioSource.PlayOneShot(levelCompleteClip);
            ShowAlert(AlertType.FinishLevel, levelCompletePanelDuration, LoadNextLevel);
            LevelCompleteEvent.Invoke();
        }

        /// <summary>
        /// Reset the player and balls
        /// </summary>
        private void LoadNextLevel()
        {
            if(levelLoader.LoadNextLevelPlease() == false)
            {
                GameComplete();
            }
        }

        /// <summary>
        /// Handle Level Start
        /// </summary>
        public void LevelStart(LevelDataExt levelData)
        {
            ShowAlert(AlertType.StartLevel, levelStartPanelDuration, null, levelData.levelName);
        }

        /// <summary>
        /// Handles Game Complete
        /// </summary>
        public void GameComplete()
        {
            GameCompleteEvent.Invoke();
            _audioSource.PlayOneShot(gameCompletedClip);
            ShowAlert(AlertType.GameComplete, gameCompletePanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Handle GameOver
        /// </summary>
        public void GameOver()
        {
            GameOverEvent.Invoke();
            _audioSource.PlayOneShot(gameOverClip);
            ShowAlert(AlertType.GameOver, gameOverPanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Check if we have new high scores
        /// </summary>
        private void CheckHighScores()
        {
            int playerOneScore = playerManager.playerOne.Score;
            int playerTwoScore = playerManager.playerTwo.Score;

            if (highScores.IsHighScore(playerOneScore))
            {
                // Debug.Log("Player 1 has a new high score");
                NewHighScore(playerManager.playerOne);
            }
            else if (highScores.IsHighScore(playerTwoScore))
            {
                // Debug.Log("Player 2 has a new high score");
                NewHighScore(playerManager.playerTwo);
            }
            else
            {
                // Debug.Log("No new high score");
                ReturnToMainMenu();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void NewHighScore(Player player)
        {
            newHighScoreInput.Show(player);
        }

        /// <summary>
        /// High score submitted button handler
        /// </summary>
        public void HighScoreSubmitted(Player player, string playerInitials)
        {
            // Debug.Log($"Submitting high score. Player: {{player.name}}, Initials: {playerInitials}, Score: {player.Score}");
            highScores.SubmitHighScore(playerInitials, player.Score);

            int playerTwoScore = playerManager.playerTwo.Score;
            if (player == playerManager.playerOne && highScores.IsHighScore(playerTwoScore))
            {
                NewHighScore(playerManager.playerTwo);
            }
            else
            {
                newHighScoreInput.Hide();
                ReturnToMainMenu();
            }
        }

        /// <summary>
        /// Show alert
        /// </summary>
        /// <param name="alertType"></param>
        /// <param name="duration"></param>
        /// <param name="actionDelegate"></param>
        /// <param name="levelName"></param>
        private void ShowAlert(AlertType alertType, float duration, Action actionDelegate, string levelName="")
        {
            infoPanel.SetActive(true);
            TextColorCycler cycler;
            switch (alertType)
            {
                case AlertType.StartLevel:
                    startLevel.SetActive(true);
                    startLevelNameText.text = levelName;
                    cycler = startLevel.GetComponentInChildren<TextColorCycler>();
                    cycler.StartColor();
                    break;
                case AlertType.FinishLevel:
                    finishLevel.SetActive(true);
                    cycler = finishLevel.GetComponentInChildren<TextColorCycler>();
                    cycler.StartColor();
                    break;
                case AlertType.GameOver:
                    gameOver.SetActive(true);
                    cycler = gameOver.GetComponentInChildren<TextColorCycler>();
                    cycler.StartColor();
                    break;
                case AlertType.GameComplete:
                    gameComplete.SetActive(true);
                    cycler = gameComplete.GetComponentInChildren<TextColorCycler>();
                    cycler.StartColor();
                    break;
            }

            StartCoroutine(HideAlertAfterDelayAsync(duration, actionDelegate));
        }

        /// <summary>
        /// Hide any alerts
        /// </summary>
        private void HideAlert()
        {
            infoPanel.SetActive(false);
            startLevel.SetActive(false);
            finishLevel.SetActive(false);
            gameOver.SetActive(false);
        }

        /// <summary>
        /// Hides alerts after a delay then runs the delegate
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideAlertAfterDelayAsync(float duration, Action actionDelegate)
        {
            // Call event and wait a frame
            BeforeAlertEvent.Invoke();
            yield return null;

            // Wait for seconds
            yield return new WaitForSecondsRealtime(duration);
            
            HideAlert();

            // Call event
            AfterAlertEvent.Invoke();

            if (actionDelegate != null)
            {
                actionDelegate.Invoke();
            }
        }

        /// <summary>
        /// Return to the Main Menu scene
        /// </summary>
        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
