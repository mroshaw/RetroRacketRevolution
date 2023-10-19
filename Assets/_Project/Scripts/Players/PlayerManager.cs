using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class PlayerManager : MonoBehaviour
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

        public bool PlayerTwoIsActive => playerTwo.gameObject.activeSelf;

        private LifeForce _lifeForce;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _lifeForce = GetComponent<LifeForce>();
        }

        /// <summary>
        /// Set up components
        /// </summary>
        private void Start()
        {
            ConfigurePlayerArea(GameController.Instance.IsTwoPlayer);
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
        /// Setup the desired control scheme when the player controller is started.
        /// </summary>
        /// <param name="input"></param>
        public void OnPlayerJoined(PlayerInput input)
        {
            Debug.Log($"Player has joined: {input.gameObject}");

            switch (input.gameObject.name)
            {
                case "Player 1":
                    if (GameController.Instance != null)
                    {
                        SetControlScheme(playerOneInput, GameController.Instance.PlayerOneControlScheme);
                    }
                    break;

                case "Player 2":
                    if (GameController.Instance != null)
                    {
                        SetControlScheme(playerTwoInput, GameController.Instance.PlayerTwoControlScheme);
                    }

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
