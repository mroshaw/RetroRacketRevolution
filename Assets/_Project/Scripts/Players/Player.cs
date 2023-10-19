using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Balls;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public enum HardPointLocation { Outer, Center, Bottom }
    
    public class Player : MonoBehaviour
    {
        // Public settings
        [BoxGroup("Player Settings")] public Vector2 defaultBatScale;
        [BoxGroup("Player Settings")] public Explosion explosion;
        [BoxGroup("Bat Settings")] public float sizeChangeFactor = 0.3f;
        [BoxGroup("Bat Settings")] public float minLength = 1.3f;
        [BoxGroup("Bat Settings")] public float maxLength = 0.5f;
        [BoxGroup("Ball Settings")] public Transform defaultBallAttachPoint;
        [BoxGroup("Ball Settings")] public bool spawnBallOnStart = false;
        [BoxGroup("Audio Settings")] public AudioClip deathClip;
        [BoxGroup("Object Settings")] public SpriteRenderer playerSprite;
        [BoxGroup("Add On Settings")] public HardPoint leftHardPoint;
        [BoxGroup("Add On Settings")] public Transform leftHardPointTransform;
        [BoxGroup("Add On Settings")] public HardPoint centerHardPoint;
        [BoxGroup("Add On Settings")] public Transform centerHardPointTransform;
        [BoxGroup("Add On Settings")] public HardPoint rightHardPoint;
        [BoxGroup("Add On Settings")] public Transform rightHardPointTransform;
        [BoxGroup("Add On Settings")] public HardPoint bottomHardPoint;
        [BoxGroup("Add On Settings")] public Transform bottomHardPointTransform;

        // Events
        [FoldoutGroup("Events")] public UnityEvent DestroyedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> ScoreUpdatedEvent;
        [FoldoutGroup("Events")] public UnityEvent PlayerResetEvent;
        [FoldoutGroup("Events")] public UnityEvent<Player> PlayerHitEvent;

        [BoxGroup("Cheats")]
        [Button("Grow Bat")]
        public void GrowBatCheat()
        {
            GrowBat();
        }
        [BoxGroup("Cheats")]
        [Button("Shrink Bat")]
        public void ShrinkBatCheat()
        {
            ShrinkBat();
        }
        [BoxGroup("Cheats")]
        [Button("Restore Bat")]
        public void RestoreBatCheat()
        {
            SetDefaultBatSize();
        }
        
        // Public properties


        // Player score
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                ScoreUpdatedEvent.Invoke(value);
            }
        }

        // Private pointers
        private List<Ball> _attachedBalls = new List<Ball>();
        private Transform _ballAttachPoint;
        private AudioSource _audioSource;

        // Private properties
        private int _score = 0;
        private Vector2 _batLengthScale;
        private GameObject _playerSpriteGameObject;

        private PlayerManager _playerManager;

        /// <summary>
        /// Init components
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _playerSpriteGameObject = playerSprite.gameObject;
            _playerManager = GetComponentInParent<PlayerManager>();
            SetDefaultBatSize();
            // Init score
            Score = 0;
        }

        /// <summary>
        /// Initialise the player
        /// </summary>
        private void Start()
        {
            // Spawn the ball, if set
            if (spawnBallOnStart)
            {
                SpawnBall();
            }
        }

        /// <summary>
        /// Is ball attached to player?
        /// </summary>
        private bool AreBallsAttached => _attachedBalls.Count > 0;

        /// <summary>
        /// Handle the fire event
        /// </summary>
        public void Fire()
        {
            // If there's a ball attached
            if (AreBallsAttached)
            {
                foreach (Ball ball in _attachedBalls.ToArray())
                {
                    ball.Detach(this);
                }
            }
        }

        /// <summary>
        /// Player has been hit
        /// </summary>
        public void Hit()
        {
            PlayerHitEvent.Invoke(this);
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        public void Kill()
        {
            explosion.Explode();
            DestroyedEvent.Invoke();
        }

        /// <summary>
        /// Spawns a new Ball prefab
        /// </summary>
        private void SpawnBall()
        {
            if (_attachedBalls.Count == 0)
            {
                Ball newBall = BallManager.Instance.GetNewBall();
                newBall.Attach(this, defaultBallAttachPoint.position);
            }
        }

        /// <summary>
        /// Attach the ball
        /// </summary>
        /// <param name="ball"></param>
        public void AttachBall(Ball ball)
        {
            _attachedBalls.Add(ball);
        }

        /// <summary>
        /// Detach the ball
        /// </summary>
        public void DetachBall(Ball ball)
        {
            _attachedBalls.Remove(ball);
        }

        /// <summary>
        /// Sets the size of the bat GameObject
        /// </summary>
        private void SetBatSize()
        {
            _playerSpriteGameObject.transform.localScale = _batLengthScale;

            // Reposition the hardpoints
            leftHardPoint.gameObject.transform.position = leftHardPointTransform.position;
            centerHardPoint.gameObject.transform.position = centerHardPointTransform.position;
            rightHardPoint.gameObject.transform.position = rightHardPointTransform.position;
        }

        /// <summary>
        /// Grow the bat
        /// </summary>
        public void GrowBat()
        {
            float newLength = _batLengthScale.x + (_batLengthScale.x * sizeChangeFactor);
            // Debug.Log($"GrowBat: New Length will be: {newLength}");
            if (newLength < maxLength)
            {
                _batLengthScale = new Vector2(newLength, _batLengthScale.y);
                SetBatSize();
            }
        }

        /// <summary>
        /// Shrink the bat
        /// </summary>
        public void ShrinkBat()
        {
            float newLength = _batLengthScale.x - (_batLengthScale.x * sizeChangeFactor);
            // Debug.Log($"ShrinkBat: New Length will be: {newLength}");
            if (newLength > minLength)
            {
                _batLengthScale = new Vector2(newLength, _batLengthScale.y);
                SetBatSize();
            }
        }

        /// <summary>
        /// Set the bat to default size
        /// </summary>
        public void SetDefaultBatSize()
        {
            _batLengthScale = defaultBatScale;
            SetBatSize();
        }

        /// <summary>
        /// Deactivate all hardpoint add-ons
        /// </summary>
        private void DeactivateHardPoints()
        {
            leftHardPoint.DisableAddOn();
            rightHardPoint.DisableAddOn();
            centerHardPoint.DisableAddOn();
        }

        /// <summary>
        /// Return a free hard points
        /// </summary>
        /// <returns></returns>
        public HardPoint GetFreeHardPoint(HardPointLocation location)
        {
            switch (location)
            {
                case HardPointLocation.Outer:
                    // If left is free, or both are occupied, use the left
                    return !leftHardPoint.IsHardPointEnabled ||
                           (leftHardPoint.IsHardPointEnabled && rightHardPoint.IsHardPointEnabled)
                        ? leftHardPoint
                        :
                        // Otherwise, use the right
                        rightHardPoint;

                case HardPointLocation.Center:
                    return centerHardPoint;
                case HardPointLocation.Bottom:
                    return bottomHardPoint;
            }
            return null;
        }

        /// <summary>
        /// Add score to player
        /// </summary>
        /// <param name="score"></param>
        public void AddScore(int score)
        {
            Score += score;
        }


        /// <summary>
        /// Resets the player
        /// </summary>
        /// <param name="spawnBall"></param>
        public void ResetPlayer(bool spawnBall)
        {
            // Reset Y position
            gameObject.transform.localPosition = new Vector2(gameObject.transform.position.x, 0.0f);
            DeactivateHardPoints();
            SetDefaultBatSize();
            if (spawnBall)
            {
                SpawnBall();
            }
        }
    }
}
