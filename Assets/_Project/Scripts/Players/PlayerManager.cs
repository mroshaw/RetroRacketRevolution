using DaftAppleGames.RetroRacketRevolution.Game;
using DaftApplesGames.RetroRacketRevolution.Balls;
using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public class PlayerManager : MonoBehaviour, IBonusRecipient
    {
        [BoxGroup("Players")] public Player playerOne;
        [BoxGroup("Players")] public Player playerTwo;
        [BoxGroup("Player Input Settings")] public PlayerInput playerOneInput;
        [BoxGroup("Player Input Settings")] public PlayerInput playerTwoInput;
        [BoxGroup("Bound Settings")] public GameObject playerOneBounds;
        [BoxGroup("Bound Settings")] public GameObject playerTwoBounds;
        [BoxGroup("Player 1 Bound Settings")] public float player1MinX;
        [BoxGroup("Player 1 Bound Settings")] public float player1MaxX;
        [BoxGroup("Player 2 Bound Settings")] public float player2MinX;
        [BoxGroup("Player 2 Bound Settings")] public float player2MaxX;
        [BoxGroup("Game Data")] public GameData gameData;
        [BoxGroup("Managers")] public BallManager ballManager;

        public bool PlayerTwoIsActive => playerTwo.gameObject.activeSelf;

        private LifeForce _lifeForce;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _lifeForce = GetComponent<LifeForce>();

            playerOne.defaultBatScale =
                new Vector2(gameData.difficulty.defaultBatLength, playerOne.defaultBatScale.y);

            playerTwo.defaultBatScale =
                new Vector2(gameData.difficulty.defaultBatLength, playerTwo.defaultBatScale.y);
        }

        /// <summary>
        /// Set up components
        /// </summary>
        private void Start()
        {
            ConfigurePlayerArea(gameData.isTwoPlayer);
            _lifeForce.NumLives = gameData.difficulty.startingLives;
            
            // Enable player 2, if appropriate
            if (gameData.isTwoPlayer)
            {
                playerOne.gameObject.SetActive(true);
                playerOne.gameObject.transform.localPosition = new Vector2(-100.0f, 0);
                playerTwo.gameObject.transform.localPosition = new Vector2(100.0f, 0);
            }
            else
            {
                playerOne.gameObject.transform.localPosition = new Vector2(0, 0);
            }
        }

        /// <summary>
        /// Set the control scheme for the given player input
        /// </summary>
        /// <param name="input"></param>
        /// <param name="controlScheme"></param>
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
        /// <param name="isTwoPlayer"></param>
        private void ConfigurePlayerArea(bool isTwoPlayer)
        {
            // Single player game
            PlayerControls playerOneControls = playerOne.GetComponent<PlayerControls>();
            if (!isTwoPlayer)
            {
                playerOneControls.minX = player1MinX;
                playerOneControls.maxX = player2MaxX;
                playerOneBounds.SetActive(false);
                playerTwoBounds.SetActive(false);

                // Position players
                Vector2 player1Position = new Vector2((player1MaxX - player1MinX) / 2, 0);

                playerOne.gameObject.transform.localPosition = player1Position;

            }
            // Two player game
            else
            {
                PlayerControls playerTwoControls = playerTwo.GetComponent<PlayerControls>();
                playerOneControls.minX = player1MinX;
                playerOneControls.maxX = player1MaxX;

                playerTwoControls.minX = player2MinX;
                playerTwoControls.maxX = player2MaxX;

                playerOneBounds.SetActive(true);
                playerTwoBounds.SetActive(true);

                // Position players
                Vector2 player1Position = new Vector2((player1MaxX - player1MinX) / 2, 0);

                Vector2 player2Position = new Vector2((player2MaxX - player2MinX) / 2, 0);
 
                playerOne.gameObject.transform.localPosition = player1Position;
                playerTwo.gameObject.transform.localPosition = player2Position;
            }
        }

        /// <summary>
        /// Spawns a ball for the player to use
        /// </summary>
        /// <returns></returns>
        public Ball SpawnNewBall()
        {
            return ballManager.GetNewBall();
        }

        /// <summary>
        /// Setup the desired control scheme when the player controller is started.
        /// </summary>
        /// <param name="input"></param>
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
        /// <param name="bonus"></param>
        /// <param name="targetGameObject"></param>
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
        public void AddLife()
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
        /// <param name="state"></param>
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
