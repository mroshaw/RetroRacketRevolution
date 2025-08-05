using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Game;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Balls
{
    public class BallManager : MonoBehaviour
    {
        [BoxGroup("Ball Settings")] [SerializeField] private GameObject ballPrefab;
        [BoxGroup("Ball Settings")] [SerializeField] private GameObject ballContainer;
        [BoxGroup("Game Data")] [SerializeField] private GameData gameData;
        [BoxGroup("Debug")] [SerializeField] private List<Ball> ballList;
        [BoxGroup("Debug")] [SerializeField] private int maxBallSpeed;

        // Events
        [BoxGroup("Events")] public UnityEvent onBallDestroyed;
        [BoxGroup("Events")] public UnityEvent onLastBallDestroyed;
        [BoxGroup("Events")] public UnityEvent<int> onBallSpeedChanged;

        /// <summary>
        /// Set up the Ball Manager
        /// </summary>
        private void Awake()
        {
            maxBallSpeed = 0;
        }

        /// <summary>
        /// Create a new ball
        /// </summary>
        internal Ball GetNewBall()
        {
            GameObject newBall = Instantiate(ballPrefab);
            newBall.transform.SetParent(ballContainer.transform, true);
            Ball ball = newBall.GetComponent<Ball>();
            ball.DefaultParent = ballContainer;

            // Configure per difficulty settings
            ball.ReconfigureBall(gameData.difficulty.defaultBallSpeed, gameData.difficulty.ballSpeedUpAfterDuration,
                gameData.difficulty.ballSpeedMultiplier);

            ball.onDestroyed.AddListener(DestroyBall);
            ball.onSpeedMultiplierChanged.AddListener(BallSpeedChanged);
            ballList.Add(ball);
            newBall.SetActive(true);
            return newBall.GetComponent<Ball>();
        }

        /// <summary>
        /// A ball is destroyed
        /// </summary>
        private void DestroyBall(Ball ball)
        {
            ballList.Remove(ball);
            Destroy(ball.gameObject);
            onBallDestroyed.Invoke();

            if (ballList.Count == 0)
            {
                onLastBallDestroyed.Invoke();
            }
        }

        /// <summary>
        /// Determine if this is the new fastest ball
        /// </summary>
        private void BallSpeedChanged(Ball ball)
        {
            int maxSpeedMultiplier = ball.CurrSpeedMultiplier;

            foreach (Ball currBall in ballList)
            {
                if (currBall.CurrSpeedMultiplier > maxSpeedMultiplier)
                {
                    maxSpeedMultiplier = currBall.CurrSpeedMultiplier;
                }
            }

            // Debug.Log($"Ball speed change detected! Max speed multiplier is: {_maxSpeedMultiplier}");
            maxBallSpeed = maxSpeedMultiplier;
            onBallSpeedChanged.Invoke(maxSpeedMultiplier);
        }

        /// <summary>
        /// Freezes all balls in place
        /// </summary>
        internal void FreezeAllBalls()
        {
            foreach (Ball ball in ballList)
            {
                ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            }
        }

        /// <summary>
        /// Unfreezes all balls
        /// </summary>
        internal void UnfreezeAllBalls()
        {
            foreach (Ball ball in ballList)
            {
                ball.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
        }

        /// <summary>
        /// Destroy all balls
        /// </summary>
        public void DestroyAllBalls()
        {
            foreach (Ball ball in ballList.ToArray())
            {
                if (!ball.IsAttached())
                {
                    ballList.Remove(ball);
                    Destroy(ball.gameObject);
                }
            }
        }

        /// <summary>
        /// Spawns 3 balls from all current balls
        /// </summary>
        internal void SpawnTripleBall()
        {
            foreach (Ball ball in ballList.ToArray())
            {
                SpawnChildBall(ball, new Vector2(0.5f, 0.0f), Vector2.right + Vector2.up);
                SpawnChildBall(ball, new Vector2(-0.5f, 0.0f), Vector2.left + Vector2.up);
                SpawnChildBall(ball, new Vector2(0.0f, 0.5f), Vector2.up);
            }
        }

        /// <summary>
        /// Make all active balls MegaBalls
        /// </summary>
        internal void MakeMegaBalls()
        {
            foreach (Ball ball in ballList.ToArray())
            {
                ball.MakeMegaBall();
            }
        }

        /// <summary>
        /// Helper to create multi-ball
        /// </summary>
        private void SpawnChildBall(Ball sourceBall, Vector2 offset, Vector2 direction)
        {
            Ball newBall1 = GetNewBall();
            newBall1.transform.position = (Vector2)sourceBall.gameObject.transform.position + offset;
            newBall1.LastTouchedByPlayer = sourceBall.LastTouchedByPlayer;
            newBall1.Nudge(direction);
        }

        /// <summary>
        /// Get the position of the first ball in the list
        /// </summary>
        internal Vector2 GetBallPosition()
        {
            if (ballList.Count > 0)
            {
                return ballList[0].transform.position;
            }

            return Vector2.zero;
        }

        /// <summary>
        /// Slows all balls
        /// </summary>
        internal void SlowAllBalls()
        {
            foreach (Ball ball in ballList)
            {
                ball.SetDefaultSpeed();
            }
        }

        /// <summary>
        /// Handler the BonusApplied event
        /// </summary>
        public void BonusAppliedHandler(Bonus bonus, GameObject targetGameObject)
        {
            switch (bonus.BonusType)
            {
                case BonusType.MultiBall:
                    SpawnTripleBall();
                    break;
                case BonusType.SlowBall:
                    SlowAllBalls();
                    break;
                case BonusType.MegaBall:
                    MakeMegaBalls();
                    break;
            }
        }
    }
}