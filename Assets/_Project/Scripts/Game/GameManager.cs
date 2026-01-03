using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.AddOns;
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
    public enum AlertType
    {
        StartLevel,
        FinishLevel,
        GameOver,
        GameComplete
    }

    public class GameManager : MonoBehaviour
    {
        [BoxGroup("Managers")] [SerializeField] private BallManager ballManager;
        [BoxGroup("Managers")] [SerializeField] private BrickManager brickManager;
        [BoxGroup("Managers")] [SerializeField] private LevelLoader levelLoader;
        [BoxGroup("Managers")] [SerializeField] private AddOnManager addOnManager;
        [BoxGroup("Managers")] [SerializeField] private PlayerManager playerManager;

        [BoxGroup("Game Data")] [SerializeField] private GameData gameData;

        [BoxGroup("UI")] [SerializeField] private GameObject infoPanel;
        [BoxGroup("Start Level UI")] [SerializeField] private GameObject startLevel;
        [BoxGroup("Start Level UI")] [SerializeField] private TextMeshProUGUI startLevelNameText;
        [BoxGroup("Start Level UI")] [SerializeField] private float levelStartPanelDuration = 3.0f;
        [BoxGroup("Start Level UI")] [SerializeField] private AudioClip levelStartedClip;

        [BoxGroup("Finish Level UI")] [SerializeField] private GameObject finishLevel;
        [BoxGroup("Finish Level UI")] [SerializeField] private TextMeshProUGUI finishLevelNameText;
        [BoxGroup("Finish Level UI")] [SerializeField] private float levelCompletePanelDuration = 3.0f;
        [BoxGroup("Finish Level UI")] [SerializeField] private AudioClip levelCompleteClip;

        [BoxGroup("Game Over UI")] [SerializeField] private GameObject gameOver;
        [BoxGroup("Game Over UI")] [SerializeField] private float gameOverPanelDuration = 5.0f;
        [BoxGroup("Game Over UI")] [SerializeField] private AudioClip gameOverClip;

        [BoxGroup("Game Complete UI")] [SerializeField] private GameObject gameComplete;
        [BoxGroup("Game Complete UI")] [SerializeField] private float gameCompletePanelDuration = 5.0f;
        [BoxGroup("Game Complete UI")] [SerializeField] private AudioClip gameCompletedClip;

        [BoxGroup("High Score UI")] [SerializeField] private CircleTextInput newHighScoreInput;

        [BoxGroup("Object Tracking")] [SerializeField] private Player player1;
        [BoxGroup("Object Tracking")] [SerializeField] private Player player2;

        [FoldoutGroup("Events")] public UnityEvent onGameStart;
        [FoldoutGroup("Events")] public UnityEvent<int> onHighScoreChanged;
        [FoldoutGroup("Events")] public UnityEvent onGameOver;
        [FoldoutGroup("Events")] public UnityEvent onGameComplete;
        [FoldoutGroup("Events")] public UnityEvent onLevelComplete;
        [FoldoutGroup("Events")] public UnityEvent onLevelStart;
        [FoldoutGroup("Events")] public UnityEvent onGameBusy;
        [FoldoutGroup("Events")] public UnityEvent onGameReady;
        [FoldoutGroup("Alert Events")] public UnityEvent onBeforeAlert;
        [FoldoutGroup("Alert Events")] public UnityEvent onAfterAlert;

        public bool CheatsUsed { get; set; }

        private HighScores _highScores;
        private const string HighScore = "HighScore";

        private AudioSource _audioSource;
        private LevelDataExt _currentLevelData;

        private bool _allBricksDestroyed;
        private bool _allEnemiesDestroyed;

        private int _currLevelIndex;

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
            _currLevelIndex = 1;
        }

        /// <summary>
        /// Set up the UI
        /// </summary>
        private void Start()
        {
            // Set unlimited framerate
            Application.targetFrameRate = -1;
            _highScores = new HighScores();
            _highScores.LoadHighScores();
            GetHighScore();
            onGameStart?.Invoke();
        }

        /// <summary>
        /// Get the current high score
        /// </summary>
        private void GetHighScore()
        {
            int highScore = _highScores.GetCurrentHighScore();
            onHighScoreChanged.Invoke(highScore);
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
            if (_currentLevelData == null)
            {
                return;
            }

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
            onLevelComplete.Invoke();
            ballManager.DestroyAllBalls();
            _audioSource.PlayOneShot(levelCompleteClip);
            ShowAlert(AlertType.FinishLevel, levelCompletePanelDuration, LoadNextLevel, _currLevelIndex.ToString());
            _currLevelIndex++;
        }

        /// <summary>
        /// Load the next level
        /// </summary>
        private void LoadNextLevel()
        {
            if (levelLoader.LoadNextLevel() == false)
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
            ShowAlert(AlertType.StartLevel, levelStartPanelDuration, LevelStartReady, _currLevelIndex.ToString());
        }

        /// <summary>
        /// Trigger the level start ready event
        /// </summary>
        private void LevelStartReady()
        {
            onLevelStart?.Invoke();
        }

        /// <summary>
        /// Handles Game Complete
        /// </summary>
        public void GameComplete()
        {
            onGameComplete.Invoke();
            _audioSource.PlayOneShot(gameCompletedClip);
            ShowAlert(AlertType.GameComplete, gameCompletePanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Handle GameOver
        /// </summary>
        public void GameOver()
        {
            onGameOver.Invoke();
            _audioSource.PlayOneShot(gameOverClip);
            ShowAlert(AlertType.GameOver, gameOverPanelDuration, CheckHighScores);
        }

        /// <summary>
        /// Check if we have new high scores
        /// </summary>
        private void CheckHighScores()
        {
            int playerOneScore = playerManager.PlayerOneScore;
            int playerTwoScore = playerManager.PlayerTwoScore;

            if (_highScores.IsHighScore(playerOneScore))
            {
                // Debug.Log("Player 1 has a new high score");
                NewHighScore(playerManager.PlayerOne);
            }
            else if (_highScores.IsHighScore(playerTwoScore))
            {
                // Debug.Log("Player 2 has a new high score");
                NewHighScore(playerManager.PlayerTwo);
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

            string difficulty = gameData.difficulty.difficultyName;
            string cheatsUsed = CheatsUsed ? "Yes" : "No";

            _highScores.SubmitHighScore(playerInitials, player.Score, difficulty, levelsPlayed, cheatsUsed);

            int playerTwoScore = playerManager.PlayerTwoScore;
            if (player == playerManager.PlayerOne && _highScores.IsHighScore(playerTwoScore))
            {
                NewHighScore(playerManager.PlayerTwo);
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
        private void ShowAlert(AlertType alertType, float duration, Action actionDelegate, string levelName = "")
        {
            onGameBusy?.Invoke();
            infoPanel.SetActive(true);
            switch (alertType)
            {
                case AlertType.StartLevel:
                    startLevel.SetActive(true);
                    startLevelNameText.text = levelName;
                    break;
                case AlertType.FinishLevel:
                    finishLevel.SetActive(true);
                    finishLevelNameText.text = levelName;
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
            onGameReady?.Invoke();
        }

        /// <summary>
        /// Hides alerts after a delay then runs the delegate
        /// </summary>
        /// <returns></returns>
        private IEnumerator HideAlertAfterDelayAsync(float duration, Action actionDelegate)
        {
            // Call event and wait a frame
            onBeforeAlert.Invoke();
            yield return null;

            // Wait for seconds
            yield return new WaitForSecondsRealtime(duration);

            HideAlert();

            // Call event
            onAfterAlert.Invoke();

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