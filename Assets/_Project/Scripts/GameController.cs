using DaftApplesGames.RetroRacketRevolution.Menus;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance { get; private set; }

        [SerializeField] private bool _isTwoPlayer = false;

        [SerializeField] private string _playerOneControlScheme;
        [SerializeField] private string _playerTwoControlScheme;
        [SerializeField] public HighScores _highScores;

        /// <summary>
        /// Control Scheme selected by Player 1
        /// </summary>
        public string PlayerOneControlScheme
        {
            get => _playerOneControlScheme;
            set => _playerOneControlScheme = value;
        }

        /// <summary>
        /// Control Scheme selected by Player 2
        /// </summary>
        public string PlayerTwoControlScheme
        {
            get => _playerTwoControlScheme;
            set => _playerTwoControlScheme = value;
        }

        /// <summary>
        /// High score table
        /// </summary>
        public HighScores HighScores => _highScores;

        /// <summary>
        /// Determines the game mode
        /// </summary>
        public bool IsTwoPlayer
        {
            get => _isTwoPlayer;
            set => _isTwoPlayer = value;
        }

        /// <summary>
        /// Set up the Game Controller
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
                _playerOneControlScheme = "Mouse";
                _playerTwoControlScheme = "Keyboard";
                _highScores = new HighScores();
            }
        }

        /// <summary>
        /// Set Application level settings
        /// </summary>
        private void Start()
        {
            // Set unlimited framerate
            Application.targetFrameRate = -1;
        }

        /// <summary>
        /// Save data when component is destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (HighScores != null)
            {
                HighScores.SaveHighScores();
            }
        }
    }
}
