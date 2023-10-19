using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Balls
{
    public class BallManager : MonoBehaviour
    {
        public static BallManager Instance { get; private set; }

        [BoxGroup("Ball Settings")] public GameObject ballPrefab;
        [BoxGroup("Ball Settings")] public GameObject ballContainer;
        [BoxGroup("Debug")] [SerializeField] private List<Ball> _ballList;

        // Events
        [BoxGroup("Events")] public UnityEvent BallDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent LastBallDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent<int> BallSpeedChangedEvent;

        [SerializeField] [BoxGroup("Debug")] private int _maxBallSpeed;

        /// <summary>
        /// Set up the Ball Manager
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
            }

            _maxBallSpeed = 0;
        }
        
        /// <summary>
        /// Create a new ball
        /// </summary>
        /// <returns></returns>
        public Ball GetNewBall()
        {
            GameObject newBall = Instantiate(ballPrefab);
            newBall.transform.SetParent(ballContainer.transform, true);
            Ball ball = newBall.GetComponent<Ball>();
            ball.DefaultParent = ballContainer;
            ball.BallDestroyedEvent.AddListener(DestroyBall);
            ball.BallSpeedMultiplierChangeEvent.AddListener(BallSpeedChanged);
            _ballList.Add(ball);
            return newBall.GetComponent<Ball>();
        }

        /// <summary>
        /// A ball is destroyed
        /// </summary>
        /// <param name="ball"></param>
        public void DestroyBall(Ball ball)
        {
            _ballList.Remove(ball);
            Destroy(ball.gameObject);
            BallDestroyedEvent.Invoke();

            if (_ballList.Count == 0)
            {
                LastBallDestroyedEvent.Invoke();
            }
        }

        /// <summary>
        /// Determine if this is the new fastest ball
        /// </summary>
        /// <param name="ball"></param>
        public void BallSpeedChanged(Ball ball)
        {
            int maxSpeedMultiplier = ball.CurrSpeedMultiplier;

            foreach (Ball currBall in _ballList)
            {
                if (currBall.CurrSpeedMultiplier > maxSpeedMultiplier)
                {
                    maxSpeedMultiplier = currBall.CurrSpeedMultiplier;
                }
            }
            // Debug.Log($"Ball speed change detected! Max speed multiplier is: {_maxSpeedMultiplier}");
            _maxBallSpeed = maxSpeedMultiplier;
            BallSpeedChangedEvent.Invoke(maxSpeedMultiplier);
        }

        /// <summary>
        /// Freezes all balls in place
        /// </summary>
        public void FreezeAllBalls()
        {
            foreach (Ball ball in _ballList)
            {
                ball.GetComponent<Rigidbody2D>().isKinematic = true;
            }
        }

        /// <summary>
        /// Unfreezes all balls
        /// </summary>
        public void UnfreezeAllBalls()
        {
            foreach (Ball ball in _ballList)
            {
                ball.GetComponent<Rigidbody2D>().isKinematic = false;
            }
        }

        /// <summary>
        /// Destroy all balls
        /// </summary>
        public void DestroyAllBalls()
        {
            foreach (Ball ball in _ballList.ToArray())
            {
                if (!ball.IsAttached())
                {
                    _ballList.Remove(ball);
                    Destroy(ball.gameObject);
                }
            }
        }

        /// <summary>
        /// Spawns 3 balls from all current balls
        /// </summary>
        public void SpawnTripleBall()
        {
            foreach (Ball ball in _ballList.ToArray())
            {
                SpawnChildBall(ball, new Vector2(0.5f, 0.0f), Vector2.right + Vector2.up);
                SpawnChildBall(ball, new Vector2(-0.5f, 0.0f), Vector2.left + Vector2.up);
                SpawnChildBall(ball, new Vector2(0.0f, 0.5f), Vector2.up);
            }
        }

        /// <summary>
        /// Make all active balls MegaBalls
        /// </summary>
        public void MakeMegaBalls()
        {
            foreach (Ball ball in _ballList.ToArray())
            {
                ball.MakeMegaBall();
            }
        }

        /// <summary>
        /// Helper to create multi-ball
        /// </summary>
        /// <param name="sourceBall"></param>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
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
        /// <returns></returns>
        public Vector2 GetBallPosition()
        {
            if (_ballList.Count > 0)
            {
                return _ballList[0].transform.position;
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Slows all balls
        /// </summary>
        public void SlowAllBalls()
        {
            foreach (Ball ball in _ballList)
            {
                ball.SetDefaultSpeed();
            }
        }
    }
}
