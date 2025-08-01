using System.Collections;
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
        [BoxGroup("Players")] [SerializeField] private float respawnDelay = 2.0f;
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
            PlayerMovement playerOneControls = playerOne.GetComponent<PlayerMovement>();
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
                PlayerMovement playerTwoControls = playerTwo.GetComponent<PlayerMovement>();
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

        public void KillPlayer(Player player)
        {
            player.Kill();
            _lifeForce.LoseLife();
        }
        
        public void KillBothPlayers()
        {
            KillPlayerOne(true);
            if (gameData.isTwoPlayer)
            {
                KillPlayerTwo();
            }
            _lifeForce.LoseLife();
            StartCoroutine(RespawnAsync(playerOne, true));
            if (gameData.isTwoPlayer)
            {
                StartCoroutine(RespawnAsync(playerTwo, false));
            }
        }

        public void KillPlayerOne(bool spawnBall)
        {
            playerOne.Kill();
            if (_lifeForce.NumLives == 0)
            {
                return;
            }
            StartCoroutine(RespawnAsync(playerOne, spawnBall));
        }

        public void KillPlayerTwo()
        {
            
        }
        
        /// <summary>
        /// Respawns a player after a delay
        /// </summary>
        private IEnumerator RespawnAsync(Player player, bool spawnBall)
        {
            yield return new WaitForSeconds(respawnDelay);
            player.ResetPlayer(spawnBall);
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
        /// Toggle Unlimited lives
        /// </summary>
        public bool ToggleUnlimitedLives()
        {
            _lifeForce.UnlimitedLives = !_lifeForce.UnlimitedLives;
            return _lifeForce.UnlimitedLives;
        }

        private void ResetPlayerOne(bool spawnBall)
        {
            playerOne.ResetPlayer(spawnBall);
        }

        private void ResetPlayerTwo()
        {
            playerTwo.ResetPlayer(false);
        }
    }
}