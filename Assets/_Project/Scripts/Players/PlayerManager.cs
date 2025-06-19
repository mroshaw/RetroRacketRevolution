using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class PlayerManager : MonoBehaviour, IBonusRecipient
    {
        [BoxGroup("Players")] [SerializeField] private Player playerOne;
        [BoxGroup("Players")] [SerializeField] private Player playerTwo;
        [BoxGroup("Player Input Settings")] [SerializeField] private PlayerInput playerOneInput;
        [BoxGroup("Player Input Settings")] [SerializeField] private PlayerInput playerTwoInput;
        [BoxGroup("Player 1 Bound Settings")] [SerializeField] private float player1MinX;
        [BoxGroup("Player 1 Bound Settings")] [SerializeField] private float player1MaxX;
        [BoxGroup("Player 2 Bound Settings")] [SerializeField] private float player2MinX;
        [BoxGroup("Player 2 Bound Settings")] [SerializeField] private float player2MaxX;
        [BoxGroup("Game Data")] [SerializeField] private GameData gameData;
        [BoxGroup("Managers")] [SerializeField] private BallManager ballManager;

        internal Player PlayerOne => playerOne;
        internal Player PlayerTwo => playerTwo;

        internal int PlayerOneScore => playerOne.Score;
        internal int PlayerTwoScore => playerTwo.Score;

        private bool PlayerTwoIsActive => playerTwo.gameObject.activeSelf;

        private LifeForce _lifeForce;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _lifeForce = GetComponent<LifeForce>();

            if (playerOne)
            {
                playerOne.PlayerManager = this;
                playerOne.SetBatScale(gameData.difficulty.defaultBatLength);
            }

            if (playerTwo)
            {
                playerTwo.PlayerManager = this;
                playerTwo.SetBatScale(gameData.difficulty.defaultBatLength);
            }
        }

        /// <summary>
        /// Set up components
        /// </summary>
        private void Start()
        {
            ConfigurePlayerArea(gameData.isTwoPlayer);
            _lifeForce.NumLives = gameData.difficulty.startingLives;
        }

        /// <summary>
        /// Set the control scheme for the given player input
        /// </summary>
        private void SetControlScheme(PlayerInput input, string controlScheme)
        {
            switch (controlScheme)
            {
                case "Keyboard":
                    input.SwitchCurrentControlScheme(controlScheme, Keyboard.current);
                    break;
                case "Mouse":
                    input.SwitchCurrentControlScheme(controlScheme, Pointer.current);
                    break;
                case "Gamepad":
                    input.SwitchCurrentControlScheme(controlScheme, Gamepad.current);
                    break;
            }
        }

        /// <summary>
        /// Sets up the player play areas
        /// </summary>
        private void ConfigurePlayerArea(bool isTwoPlayer)
        {

            Transform player1Transform = playerOne.transform;
            Transform player2Transform = playerTwo.transform;

            // Single player game
            PlayerControls playerOneControls = playerOne.GetComponent<PlayerControls>();
            if (!isTwoPlayer)
            {
                playerOneControls.ConfigurePlayer(player1MinX, player2MaxX);

                // Position players
                Vector3 player1Position = new((player1MaxX - player1MinX) / 2, player1Transform.position.y, player1Transform.position.z);
                playerOne.gameObject.transform.localPosition = player1Position;

            }
            // Two player game
            else
            {
                PlayerControls playerTwoControls = playerTwo.GetComponent<PlayerControls>();
                playerOneControls.ConfigurePlayer(player1MinX, player1MaxX);
                playerTwoControls.ConfigurePlayer(player2MinX, player2MaxX);

                // Position players
                Vector3 player1Position = new Vector3((player1MaxX - player1MinX) / 2, player1Transform.position.y, player1Transform.position.z);
                Vector3 player2Position = new Vector3((player2MaxX - player2MinX) / 2, player2Transform.position.y, player2Transform.position.z);
 
                playerOne.gameObject.transform.localPosition = player1Position;
                playerTwo.gameObject.transform.localPosition = player2Position;
            }
        }

        /// <summary>
        /// Spawns a ball for the player to use
        /// </summary>
        public Ball SpawnNewBall()
        {
            return ballManager.GetNewBall();
        }

        /// <summary>
        /// Setup the desired control scheme when the player controller is started.
        /// </summary>
        public void OnPlayerJoined(PlayerInput input)
        {
            Debug.Log($"Player has joined: {input.gameObject}");

            switch (input.gameObject.name)
            {
                case "Player 1":
                    SetControlScheme(playerOneInput,gameData.playerOneControlScheme);
                    break;

                case "Player 2":
                    SetControlScheme(playerTwoInput, gameData.playerTwoControlScheme);
                    break;
            }
        }

        /// <summary>
        /// Handler the BonusApplied event
        /// </summary>
        public void BonusAppliedHandler(Bonus bonus, GameObject targetGameObject)
        {
            switch (bonus.bonusType)
            {
                case BonusType.ExtraLife:
                    AddLife();
                    break;
            }
        }

        /// <summary>
        /// Add a life to the players
        /// </summary>
        private void AddLife()
        {
            _lifeForce.AddLife();
        }

        /// <summary>
        /// Remove a life from the players
        /// </summary>
        public void RemoveLife()
        {
            playerOne.Kill();
            if (PlayerTwoIsActive)
            {
                playerTwo.Kill();
            }
            _lifeForce.LoseLife();
        }

        /// <summary>
        /// Toggle Unlimited lives
        /// </summary>
        public bool ToggleUnlimitedLives()
        {
            _lifeForce.UnlimitedLives = !_lifeForce.UnlimitedLives;
            return _lifeForce.UnlimitedLives;
        }

        /// <summary>
        /// Resets both players
        /// </summary>
        public void ResetPlayers()
        {
            playerOne.ResetPlayer(true);
            if (PlayerTwoIsActive)
            {
                playerTwo.ResetPlayer(false);
            }
        }

        /// <summary>
        /// Cause players to zoom up the screen
        /// </summary>
        public void PlayersZoom()
        {

        }
    }
}