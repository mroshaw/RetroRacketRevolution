using System;
using System.Collections;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using DaftAppleGames.RetroRacketRevolution.Enemies;
using DaftAppleGames.RetroRacketRevolution.Players;
using DaftAppleGames.RetroRacketRevolution.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace DaftAppleGames.RetroRacketRevolution.Balls
{
    public class Ball : MonoBehaviour
    {
        // Public settings
        [BoxGroup("Ball Settings")] [SerializeField] private float defaultBallSpeed = 100.0f;
        [BoxGroup("Ball Settings")] [SerializeField] private float speedUpAfterDuration = 20.0f;
        [BoxGroup("Ball Settings")] [SerializeField] private float speedMultiplier = 1.2f;
        [BoxGroup("Ball Settings")] [SerializeField] private float spinForce = 50.0f;
        [BoxGroup("Normal Ball")] [SerializeField] private Color normalBallColor;
        [BoxGroup("Normal Ball")] [SerializeField] private Color normalBallEmissiveColor;
        [BoxGroup("Normal Ball")] [SerializeField] private float normalBallEmissive;

        [BoxGroup("MegaBall")] [SerializeField] private float megaBallDuration = 5.0f;
        [BoxGroup("MegaBall")] [SerializeField] private Color megaBallColor;
        [BoxGroup("MegaBall")] [SerializeField] private Color megaBallEmissiveColor;
        [BoxGroup("MegaBall")] [SerializeField] private float megaBallEmissive;

        [BoxGroup("Audio")] [SerializeField] private AudioClip hitPlayerClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip hitBoundaryClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip hitBrickClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip hitMultibrickClip;
        [BoxGroup("Audio")] [SerializeField] private AudioClip hitInvincibleBrickClip;

        [BoxGroup("Debug")] [SerializeField] private float speedChangeTimer;
        [BoxGroup("Debug")] [SerializeField] private float currSpeed;

        // Events
        [BoxGroup("Events")] [SerializeField] public UnityEvent onAttached;
        [BoxGroup("Events")] [SerializeField] public UnityEvent onDetached;
        [BoxGroup("Events")] [SerializeField] public UnityEvent<Ball> onDestroyed;
        [BoxGroup("Events")] [SerializeField] public UnityEvent<Ball> onSpeedMultiplierChanged;

        private Player _attachedPlayer;
        private MaterialTools _materialTools;
        private Rigidbody _rigidBody;
        private AudioSource _audioSource;

        // Used for scoring
        internal Player LastTouchedByPlayer { get; set; }

        // Public properties
        internal GameObject DefaultParent { get; set; }

        internal int CurrSpeedMultiplier { get; private set; }

        private bool _isMegaBall;

        /// <summary>
        /// Setup component references
        /// </summary>
        private void Awake()
        {
            _rigidBody = GetComponentInChildren<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            _materialTools = GetComponent<MaterialTools>();
            currSpeed = defaultBallSpeed;
            speedChangeTimer = 0;
            CurrSpeedMultiplier = 1;
        }

        /// <summary>
        /// Init the ball component
        /// </summary>
        private void Start()
        {
            onSpeedMultiplierChanged.Invoke(this);
            MakeNormalBall();
        }

        /// <summary>
        /// Implement frame update code
        /// </summary>
        private void Update()
        {
            if (IsAttached())
            {
                return;
            }

            speedChangeTimer += Time.deltaTime;
            if (speedChangeTimer > speedUpAfterDuration)
            {
                speedChangeTimer = 0;
                currSpeed *= speedMultiplier;
                CurrSpeedMultiplier++;
                onSpeedMultiplierChanged.Invoke(this);
            }

            CheckOutOfBounds();
        }

        /// <summary>
        /// Reconfigure the ball properties
        /// </summary>
        internal void ReconfigureBall(float newBallSpeed, float newSpeedUpAfterDuration, float newSpeedMultiplier)
        {
            defaultBallSpeed = newBallSpeed;
            speedUpAfterDuration = newSpeedUpAfterDuration;
            speedMultiplier = newSpeedMultiplier;
        }

        /// <summary>
        /// Check the ball hasn't fled the bounds of the game
        /// </summary>
        private void CheckOutOfBounds()
        {
            if (transform.position.x < -1000 || transform.position.x > 1000)
            {
                DestroyBall();
            }
        }

        /// <summary>
        /// Makes the ball a MegaBall
        /// </summary>
        [Button("Make Mega Ball")]
        internal void MakeMegaBall()
        {
            _rigidBody.excludeLayers |= (1 << LayerMask.NameToLayer("Bricks"));
            _materialTools.SetColor(megaBallColor);
            _materialTools.SetColor(megaBallEmissiveColor);
            _materialTools.SetEmission(megaBallEmissive);
            _isMegaBall = true;
            StartCoroutine(RevertMegaBallAsync(megaBallDuration));
        }

        /// <summary>
        /// Revert the ball to normal
        /// </summary>
        [Button("Make Normal Ball")]
        internal void MakeNormalBall()
        {
            _rigidBody.excludeLayers &= ~(1 << LayerMask.NameToLayer("Bricks"));
            _materialTools.SetColor(normalBallColor);
            _materialTools.SetColor(normalBallEmissiveColor);
            _materialTools.SetEmission(normalBallEmissive);

            _isMegaBall = false;
        }

        /// <summary>
        /// Revert to normal after number of seconds
        /// </summary>
        private IEnumerator RevertMegaBallAsync(float duration)
        {
            yield return new WaitForSeconds(duration);
            MakeNormalBall();
        }

        /// <summary>
        /// Returns if ball is attached to a player
        /// </summary>
        internal bool IsAttached()
        {
            return _attachedPlayer != null;
        }

        /// <summary>
        /// Reset the speed to default
        /// </summary>
        internal void ResetSpeed()
        {
            currSpeed = defaultBallSpeed;
            speedChangeTimer = 0;
            CurrSpeedMultiplier = 1;
            onSpeedMultiplierChanged.Invoke(this);
        }

        /// <summary>
        /// Handle collisions
        /// </summary>
        void OnCollisionEnter(Collision other)
        {
            Debug.Log($"Ball hit: {other.gameObject.name}");
            // Hit the player
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
            {
                CollideWithPlayer(other);
                return;
            }

            // Hit enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                CollideWithEnemy(enemy);
                return;
            }

            // Hit the boundary
            if (other.gameObject.CompareTag("Boundary"))
            {
                CollideWithBoundary();
                return;
            }

            // Hit a brick
            if (other.gameObject.CompareTag("Brick"))
            {
                Brick brick = other.gameObject.GetComponent<Brick>();
                CollideWithBrick(brick);
            }

            // Goes out of bounds, off the bottom of the screen
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                DestroyBall();
            }
        }

        /// <summary>
        /// Handle collision with Enemy
        /// </summary>
        private void CollideWithEnemy(Enemy enemy)
        {
            enemy.Hit(this.gameObject);
        }

        /// <summary>
        /// Handles collision with boundary
        /// </summary>
        private void CollideWithBoundary()
        {
            if (_audioSource.enabled && hitBoundaryClip)
            {
                _audioSource.PlayOneShot(hitBoundaryClip);
            }
        }

        /// <summary>
        /// Handles collision with a brick
        /// </summary>
        private void CollideWithBrick(Brick brick)
        {
            // Play appropriate sound clip
            switch (brick.BrickType)
            {
                case BrickType.Normal:
                    break;
                case BrickType.DoubleStrong:
                case BrickType.TripleStrong:
                    if (brick.Health > 1)
                    {
                        _audioSource.PlayOneShot(hitMultibrickClip);
                    }

                    break;
                case BrickType.Invincible:
                    _audioSource.PlayOneShot(hitInvincibleBrickClip);
                    break;
            }

            brick.BrickHit(LastTouchedByPlayer);
        }

        /// <summary>
        /// Handles collision with player
        /// </summary>
        private void CollideWithPlayer(Collision other)
        {
            Player player = other.gameObject.GetComponent<Player>();

            // We're just attaching the ball to the player
            if (IsAttached())
            {
                return;
            }

            LastTouchedByPlayer = player;

            // Calculate hit Factor
            float x = GetHorizontalVelocityFactor(transform.position,
                other.transform.position, other.collider.bounds.size.x);

            float y = GetVerticalVelocityFactor(transform.position,
                other.transform.position, other.collider.bounds.size.x);

            // Calculate direction, set length to 1
            Vector3 dir = new Vector3(x, y, 1).normalized;

            // Set Velocity with dir * speed
            _rigidBody.linearVelocity = dir * currSpeed;
            Spin();
            if (_audioSource.enabled)
            {
                _audioSource.PlayOneShot(hitPlayerClip);
            }
        }

        /// <summary>
        /// Check for out of bounds
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                DestroyBall();
            }
        }

        /// <summary>
        /// Calculate the horizontal angle of the bounce
        /// </summary>
        private float GetHorizontalVelocityFactor(Vector2 ballPos, Vector2 playerPos,
            float playerWidth)
        {
            float xFactor = (ballPos.x - playerPos.x) / playerWidth;
            // Debug.Log($"X factor: {xFactor}");
            return xFactor;
        }

        /// <summary>
        /// Calculate the vertical angle of the bounce
        /// </summary>
        private float GetVerticalVelocityFactor(Vector2 ballPos, Vector2 playerPos, float playerWidth)
        {
            float n = (ballPos.x - playerPos.x) / playerWidth;
            // float yFactor = n < 0 ? n - 1.0f : n + 1.0f;
            float yFactor = 1.0f + Math.Abs(n);
            // Debug.Log($"Y factor: {yFactor}");
            // return n < 0 ? n - 1.0f : n + 1.0f;
            return yFactor;
        }

        /// <summary>
        /// Request that the ball be attached to the player
        /// </summary>
        internal void Attach(Player player, Vector3 attachPosition)
        {
            if (IsAttached())
            {
                return;
            }

            _attachedPlayer = player;
            LastTouchedByPlayer = player;
            _rigidBody.angularVelocity = Vector3.zero;
            _rigidBody.linearVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;
            gameObject.transform.SetParent(player.gameObject.transform, true);
            gameObject.transform.position = attachPosition;
            player.AttachBall(this);
            onAttached.Invoke();
        }

        /// <summary>
        /// Request to detach the ball
        /// </summary>
        internal void Detach(Player player)
        {
            gameObject.transform.SetParent(DefaultParent.transform, true);
            _attachedPlayer = null;
            _rigidBody.isKinematic = false;
            speedChangeTimer = 0.0f;
            _rigidBody.linearVelocity = (Vector2.up + 0.1f * RandomVector()) * defaultBallSpeed;
            Spin();
            player.DetachBall(this);
            onDetached.Invoke();
        }

        /// <summary>
        /// Get a random left or right vector
        /// </summary>
        private Vector2 RandomVector()
        {
            return Random.Range(0, 100) < 50 ? Vector2.left : Vector2.right;
        }

        /// <summary>
        /// Nudges the ball in the given direction
        /// </summary>
        internal void Nudge(Vector2 vectorDirection)
        {
            _rigidBody.linearVelocity = vectorDirection * defaultBallSpeed;
        }

        /// <summary>
        /// Returns the ball to default speed
        /// </summary>
        internal void SetDefaultSpeed()
        {
            currSpeed = defaultBallSpeed;
            speedChangeTimer = 0.0f;
        }

        /// <summary>
        /// Destroy the ball
        /// </summary>
        private void DestroyBall()
        {
            onSpeedMultiplierChanged.Invoke(this);
            onDestroyed.Invoke(this);
        }

        /// <summary>
        /// Adds random torque to set the ball spinning
        /// </summary>
        internal void Spin()
        {
            // Generate random torque values for each axis
            float randomX = Random.Range(-spinForce, spinForce);
            float randomY = Random.Range(-spinForce, spinForce);
            float randomZ = Random.Range(-spinForce, spinForce);

            // Create a Vector3 with the random torque values
            Vector3 randomTorque = new Vector3(randomX, randomY, randomZ);

            // Apply the torque to the Rigidbody
            _rigidBody.AddTorque(randomTorque);
        }
    }
}