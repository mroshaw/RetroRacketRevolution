using System;
using System.Collections;
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

namespace DaftApplesGames.RetroRacketRevolution
{
    public enum AlertType {StartLevel, FinishLevel, GameOver, GameComplete }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

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

        private const string HighScore = "HighScore";

        private AudioSource _audioSource;

        /// <summary>
        /// Set up the Game Manager
        /// </summary>
        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                _audioSource = GetComponent<AudioSource>();
                HideAlert();
            }
        }

        /// <summary>
        /// Set up the UI
        /// </summary>
        private void Start()
        {
            GetHighScore();
            // Enable player 2, if appropriate
            if (GameController.Instance.IsTwoPlayer)
            {
                player2.gameObject.SetActive(true);
                player1.gameObject.transform.localPosition = new Vector2(-100.0f, 0);
                player2.gameObject.transform.localPosition = new Vector2(100.0f, 0);
            }
            else
            {
                player1.gameObject.transform.localPosition = new Vector2(0, 0);
            }
        }

        /// <summary>
        /// Get the current high score
        /// </summary>
        private void GetHighScore()
        {
            int highScore = GameController.Instance.HighScores.GetCurrentHighScore();
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
            Time.timeScale = 0.0f;
            ShowAlert(AlertType.GameComplete, gameCompletePanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Handle GameOver
        /// </summary>
        public void GameOver()
        {
            GameOverEvent.Invoke();
            _audioSource.PlayOneShot(gameOverClip);
            Time.timeScale = 0.0f;
            ShowAlert(AlertType.GameOver, gameOverPanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Check if we have new high scores
        /// </summary>
        private void CheckHighScores()
        {
            int playerOneScore = playerManager.playerOne.Score;
            int playerTwoScore = playerManager.playerTwo.Score;

            if (GameController.Instance.HighScores.IsHighScore(playerOneScore))
            {
                // Debug.Log("Player 1 has a new high score");
                NewHighScore(playerManager.playerOne);
            }
            else if (GameController.Instance.HighScores.IsHighScore(playerTwoScore))
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
            GameController.Instance.HighScores.SubmitHighScore(playerInitials, player.Score);

            int playerTwoScore = playerManager.playerTwo.Score;
            if (player == playerManager.playerOne && GameController.Instance.HighScores.IsHighScore(playerTwoScore))
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
            switch (alertType)
            {
                case AlertType.StartLevel:
                    startLevel.SetActive(true);
                    startLevelNameText.text = levelName;
                    break;
                case AlertType.FinishLevel:
                    finishLevel.SetActive(true);
                    break;
                case AlertType.GameOver:
                    gameOver.SetActive(true);
                    break;
                case AlertType.GameComplete:
                    gameComplete.SetActive(true);
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
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
