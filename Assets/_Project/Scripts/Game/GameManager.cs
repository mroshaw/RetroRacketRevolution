using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Effects;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using DaftAppleGames.RetroRacketRevolution.Levels;
using DaftAppleGames.RetroRacketRevolution.Menus;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace DaftAppleGames.RetroRacketRevolution.Game
{
    public enum AlertType {StartLevel, FinishLevel, GameOver, GameComplete }

    public class GameManager : MonoBehaviour
    {
        [BoxGroup("Managers")] public BallManager ballManager;
        [BoxGroup("Managers")] public BrickManager brickManager;
        [BoxGroup("Managers")] public LevelLoader levelLoader;
        [BoxGroup("Managers")] public AddOnManager addOnManager;
        [BoxGroup("Managers")] public PlayerManager playerManager;

        [BoxGroup("Game Data")] public GameData gameData;

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

        public bool CheatsUsed { get; set; }

        private HighScores highScores;

        private const string HighScore = "HighScore";

        private AudioSource _audioSource;
        private LevelDataExt _currentLevelData;

        private bool _allBricksDestroyed;
        private bool _allEnemiesDestroyed;

        /// <summary>
        /// Set up the Game Manager
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            HideAlert();
            CheatsUsed = false;
            _allBricksDestroyed = false;
            _allEnemiesDestroyed = false;
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
        /// Public setter for All Bricks Destroyed
        /// Used in Events to tell GameManager
        /// </summary>
        public void SetAllBricksDestroyed()
        {
            _allBricksDestroyed = true;
            CheckLevelComplete();
        }

        /// <summary>
        /// Public setter for All Enemies Destroyed
        /// Used in Events to tell GameManager that boss is dead
        /// </summary>
        public void SetAllEnemiesDestroyed()
        {
            _allEnemiesDestroyed = true;
            CheckLevelComplete();
        }

        /// <summary>
        /// Check to see if the level is complete
        /// </summary>
        public void CheckLevelComplete()
        {
            // In Boss Level and boss has been destroyed
            if (_currentLevelData.isBossLevel && _allBricksDestroyed && _allEnemiesDestroyed)
            {
                LevelComplete();
                return;
            }

            // Not in boss level and all bricks have been destroyed
            if (!_currentLevelData.isBossLevel && _allBricksDestroyed)
            {
                LevelComplete();
            }
        }

        /// <summary>
        /// Handle Level Complete
        /// </summary>
        public void LevelComplete()
        {
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
            if(levelLoader.LoadNextLevel() == false)
            {
                GameComplete();
            }
        }

        /// <summary>
        /// Handle Level Start
        /// </summary>
        public void LevelStart(LevelDataExt levelData)
        {
            _currentLevelData = levelData;
            _allBricksDestroyed = false;
            _allEnemiesDestroyed = false;
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

            // Determine values to send to High Score system
            string levelsPlayed = "Original";
            string difficulty = "Normal";
            string cheatsUsed = "No";

            // Get "Friendly" name for levels selected.
            switch (gameData.levelSelect)
            {
                case LevelSelect.Original:
                    levelsPlayed = "Original";
                    break;
                case LevelSelect.Custom:
                    levelsPlayed = "Custom";
                    break;
                case LevelSelect.OgPlusCustom:
                case LevelSelect.CustomPlusOg:
                    levelsPlayed = "All";
                    break;
            }

            difficulty = gameData.difficulty.difficultyName;
            cheatsUsed = CheatsUsed ? "Yes" : "No";

            highScores.SubmitHighScore(playerInitials, player.Score,difficulty, levelsPlayed, cheatsUsed);

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
