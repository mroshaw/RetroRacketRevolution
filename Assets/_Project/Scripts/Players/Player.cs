using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Effects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public enum HardPointLocation
    {
        Outer,
        Center,
        Bottom
    }

    public class Player : MonoBehaviour, IBonusRecipient
    {
        // Public settings
        [BoxGroup("Player Settings")] [SerializeField] private Vector3 defaultBatScale;
        [BoxGroup("Player Settings")] [SerializeField] private Color shipColor;
        [BoxGroup("Player Settings")] [SerializeField] private Vector3 startPosition;
        [BoxGroup("Bat Settings")] [SerializeField] private float sizeChangeFactor = 0.3f;
        [BoxGroup("Bat Settings")] [SerializeField] private float minLength = 1.3f;
        [BoxGroup("Bat Settings")] [SerializeField] private float maxLength = 0.5f;
        [BoxGroup("Ball Settings")] [SerializeField] private Transform defaultBallAttachPoint;
        [BoxGroup("Ball Settings")] [SerializeField] private bool spawnBallOnStart;
        [BoxGroup("Audio")] [SerializeField] private AudioClip launchBallClip;
        [BoxGroup("Death")] [SerializeField] private Explosion explosion;
        [BoxGroup("Object Settings")] [SerializeField] private GameObject playerModelGameObject;
        [BoxGroup("Add On Settings")] [SerializeField] private HardPoint leftHardPoint;
        [BoxGroup("Add On Settings")] [SerializeField] private HardPoint centerHardPoint;
        [BoxGroup("Add On Settings")] [SerializeField] private HardPoint rightHardPoint;
        [BoxGroup("Add On Settings")] [SerializeField] private HardPoint bottomHardPoint;

        // Events
        [FoldoutGroup("Events")] public UnityEvent onDestroyed;
        [FoldoutGroup("Events")] public UnityEvent<int> onScoreUpdated;
        [FoldoutGroup("Events")] public UnityEvent onReset;
        [FoldoutGroup("Events")] public UnityEvent<Player> onHit;

        internal bool Destroyed { get; private set; }


        // Player score
        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                onScoreUpdated.Invoke(value);
            }
        }

        // Private pointers
        private readonly List<Ball> _attachedBalls = new List<Ball>();
        private Transform _ballAttachPoint;

        // Private properties
        private int _score;
        private Vector3 _batLengthScale;
        private PlayerManager _playerManager;
        private AudioSource _audioSource;

        internal PlayerManager PlayerManager
        {
            set => _playerManager = value;
        }

        /// <summary>
        /// Init components
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            // Init score
            Score = 0;
        }

        /// <summary>
        /// Initialise the player
        /// </summary>
        private void Start()
        {
            // SetDefaultBatSize();

            // Spawn the ball, if set
            if (spawnBallOnStart)
            {
                SpawnBall();
            }
        }

        private void SetShipColor()
        {
        }

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

        /// <summary>
        /// Sets the player bat scale
        /// </summary>
        internal void SetBatScale(float newScale)
        {
            defaultBatScale = new Vector3(newScale, defaultBatScale.y, defaultBatScale.z);
        }


        /// <summary>
        /// Is ball attached to player?
        /// </summary>
        private bool AreBallsAttached => _attachedBalls.Count > 0;

        public void BeginFiring()
        {
            // If there's a ball attached
            if (AreBallsAttached)
            {
                _audioSource.PlayOneShot(launchBallClip);
                foreach (Ball ball in _attachedBalls.ToArray())
                {
                    ball.Detach(this);
                }
            }

            // Fire hardpoints
            if (leftHardPoint.IsDeployed)
            {
                leftHardPoint.FirePressed();
            }

            if (rightHardPoint.IsDeployed)
            {
                rightHardPoint.FirePressed();
            }
        }

        public void EndFiring()
        {
            // End fire hardpoints
            if (leftHardPoint.IsDeployed)
            {
                leftHardPoint.FireReleased();
            }

            if (rightHardPoint.IsDeployed)
            {
                rightHardPoint.FireReleased();
            }
        }

        /// <summary>
        /// Player has been hit
        /// </summary>
        public void Hit()
        {
            onHit.Invoke(this);
        }

        /// <summary>
        /// Kill the player
        /// </summary>
        public void Kill()
        {
            Destroyed = true;
            playerModelGameObject.SetActive(false);
            explosion.Explode(true);
            onDestroyed.Invoke();
        }

        /// <summary>
        /// Spawns a new Ball prefab
        /// </summary>
        private void SpawnBall()
        {
            if (!_playerManager)
            {
                return;
            }

            if (_attachedBalls.Count == 0)
            {
                Ball newBall = _playerManager.SpawnNewBall();
                newBall.Attach(this, defaultBallAttachPoint.position);
            }
        }

        /// <summary>
        /// Handle Bonus Applied events
        /// </summary>
        public void ApplyBonus(Bonus bonus)
        {
            switch (bonus.BonusType)
            {
                case BonusType.SmallScore:
                case BonusType.BigScore:
                    AddScore(bonus.ScoreToAdd);
                    break;
                case BonusType.Laser:
                case BonusType.Catcher:
                case BonusType.FinishLevel:
                    HardPoint playerHardPoint = GetFreeHardPoint(bonus.HardPointLocation);
                    if (playerHardPoint != null)
                    {
                        playerHardPoint.Deploy();
                        if (bonus.Duration > 0.0f)
                        {
                            playerHardPoint.DeactivateHardPointAfterDelay(bonus.Duration);
                        }
                    }

                    break;
                case BonusType.ExtraLife:
                    _playerManager.AddLife();
                    break;
                case BonusType.ShrinkBat:
                    ShrinkBat();
                    break;
                case BonusType.GrowBat:
                    GrowBat();
                    break;
                case BonusType.MegaBall:
                    _playerManager.MakeMegaBalls();
                    break;
                case BonusType.MultiBall:
                    _playerManager.MakeMultiBalls();
                    break;
                case BonusType.SlowBall:
                    _playerManager.MakeSlowBalls();
                    break;
            }
        }

        /// <summary>
        /// Attach the ball
        /// </summary>
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
            // playerModelGameObject.transform.localScale = _batLengthScale;

            // Reposition the hardpoints
            // leftHardPoint.gameObject.transform.position = leftHardPointTransform.position;
            // centerHardPoint.gameObject.transform.position = centerHardPointTransform.position;
            // rightHardPoint.gameObject.transform.position = rightHardPointTransform.position;
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
                _batLengthScale = new Vector3(newLength, _batLengthScale.y, _batLengthScale.z);
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
                _batLengthScale = new Vector3(newLength, _batLengthScale.y, _batLengthScale.z);
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
            leftHardPoint.Retract();
            rightHardPoint.Retract();
            centerHardPoint.Retract();
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
                    return !leftHardPoint.IsDeployed ||
                           (leftHardPoint.IsDeployed && rightHardPoint.IsDeployed)
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
        public void ResetPlayer(bool spawnBall)
        {
            Debug.Log("Resetting Player!");
            transform.position = startPosition;
            playerModelGameObject.SetActive(true);
            DeactivateHardPoints();
            SetDefaultBatSize();
            if (spawnBall)
            {
                SpawnBall();
            }

            Destroyed = false;

            onReset?.Invoke();
        }
    }
}