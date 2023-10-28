using System;
using System.Collections;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Balls
{
    public class Ball : MonoBehaviour
    {
        // Public settings
        [BoxGroup("Ball Settings")] public float defaultBallSpeed = 100.0f;
        [BoxGroup("Ball Settings")] public float speedUpAfterDuration = 20.0f;
        [BoxGroup("Ball Settings")] public float speedMultiplier = 1.2f;

        [BoxGroup("MegaBall")] public SpriteRenderer normalBallSpriteRenderer;
        [BoxGroup("MegaBall")] public SpriteRenderer megaBallSpriteRenderer;
        [BoxGroup("MegaBall")] public float megaBallDuration = 5.0f;

        [BoxGroup("Audio")] public AudioClip hitPlayerClip;
        [BoxGroup("Audio")] public AudioClip hitBoundaryClip;
        [BoxGroup("Audio")] public AudioClip hitBrickClip;
        [BoxGroup("Audio")] public AudioClip hitMultibrickClip;
        [BoxGroup("Audio")] public AudioClip hitInvincibleBrickClip;

        // Events
        [BoxGroup("Events")] public UnityEvent<Ball> BallDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent<Ball> BallSpeedMultiplierChangeEvent;

        // Private pointers
        private Player _attachedPlayer;

        // Used for scoring
        public Player LastTouchedByPlayer { get; set; }

        // Public properties
        public GameObject DefaultParent { get; set; }

        public int CurrSpeedMultiplier { get; private set; }


        // Components
        private Rigidbody2D _rb;
        private AudioSource _audioSource;
        private TrailRenderer _trailRenderer;

        private PhysicsMaterial2D _physicsMaterial;

        [SerializeField]
        private float _speedChangeTimer;
        [SerializeField]
        private float _currSpeed;
        
        /// <summary>
        /// Setup component references
        /// </summary>
        private void Awake()
        {
            _rb = GetComponentInChildren<Rigidbody2D>();
            _audioSource = GetComponent<AudioSource>();
            _trailRenderer = normalBallSpriteRenderer.gameObject.GetComponentInChildren<TrailRenderer>(true);
            _currSpeed = defaultBallSpeed;
            _speedChangeTimer = 0;
            CurrSpeedMultiplier = 1;

            SetNormalSprite();
            _physicsMaterial = _rb.sharedMaterial;
        }

        /// <summary>
        /// Init the ball component
        /// </summary>
        private void Start()
        {
            BallSpeedMultiplierChangeEvent.Invoke(this);
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
            _speedChangeTimer += Time.deltaTime;
            if (_speedChangeTimer > speedUpAfterDuration)
            {
                _speedChangeTimer = 0;
                _currSpeed *= speedMultiplier;
                CurrSpeedMultiplier++;
                BallSpeedMultiplierChangeEvent.Invoke(this);
            }
            CheckOutOfBounds();
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
        /// <param name="duration"></param>
        [Button("Make Mega Ball")]
        public void MakeMegaBall()
        {
            SetMegaBallSprite();
            _rb.excludeLayers |= (1 << LayerMask.NameToLayer("Bricks"));
            StartCoroutine(RevertMegaBallAsync(megaBallDuration));
        }

        /// <summary>
        /// Revert the ball to normal
        /// </summary>
        [Button("Make Normal Ball")]
        public void MakeNormalBall()
        {
            SetNormalSprite();
            _rb.excludeLayers &= ~(1 << LayerMask.NameToLayer("Bricks"));
        }

        /// <summary>
        /// Revert to normal after number of seconds
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private IEnumerator RevertMegaBallAsync(float duration)
        {
            yield return new WaitForSeconds(duration);
            MakeNormalBall();
        }

        /// <summary>
        /// Change the sprite renderer to normal
        /// </summary>
        private void SetNormalSprite()
        {
            normalBallSpriteRenderer.gameObject.SetActive(true);
            megaBallSpriteRenderer.gameObject.SetActive(false);
        }

        /// <summary>
        /// Change the sprite renderer to the mega ball
        /// </summary>
        private void SetMegaBallSprite()
        {
            normalBallSpriteRenderer.gameObject.SetActive(false);
            megaBallSpriteRenderer.gameObject.SetActive(true);
        }

        /// <summary>
        /// Returns if ball is attached to a player
        /// </summary>
        /// <returns></returns>
        public bool IsAttached()
        {
            return _attachedPlayer != null;
        }

        /// <summary>
        /// Reset the speed to default
        /// </summary>
        public void ResetSpeed()
        {
            _currSpeed = defaultBallSpeed;
            _speedChangeTimer = 0;
            CurrSpeedMultiplier = 1;
            BallSpeedMultiplierChangeEvent.Invoke(this);
        }

        /// <summary>
        /// Handle collisions
        /// </summary>
        /// <param name="col"></param>
        void OnCollisionEnter2D(Collision2D other)
        {
            // Hit the player
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
            {
                CollideWithPlayer(other);
            }

            // Hit enemy
            if (other.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = other.gameObject.GetComponent<Enemy>();
                CollideWithEnemy(enemy);
            }

            // Hit the boundary
            if (other.gameObject.CompareTag("Boundary"))
            {
                CollideWithBoundary();
            }

            // Hit a brick
            if (other.gameObject.CompareTag("Brick"))
            {
                Brick brick = other.gameObject.GetComponent<Brick>();
                CollideWithBrick(brick);
            }
        }

        /// <summary>
        /// Handle collision with Enemy
        /// </summary>
        /// <param name="enemy"></param>
        private void CollideWithEnemy(Enemy enemy)
        {
            enemy.Hit(this.gameObject);
        }

        /// <summary>
        /// Handles collision with boundary
        /// </summary>
        private void CollideWithBoundary()
        {
            if (_audioSource.enabled)
            {
                _audioSource.PlayOneShot(hitBoundaryClip);
            }
        }
        
        /// <summary>
        /// Handles collision with a brick
        /// </summary>
        /// <param name="other"></param>
        public void CollideWithBrick(Brick brick)
        {
            // Play appropriate sound clip
            switch (brick.brickType)
            {
                case BrickType.Normal:
                    _audioSource.PlayOneShot(hitBrickClip);
                    break;
                case BrickType.DoubleStrong:
                case BrickType.TripleStrong:
                    if (brick.Health > 1)
                    {
                        _audioSource.PlayOneShot(hitMultibrickClip);
                    }
                    else
                    {
                        _audioSource.PlayOneShot(hitBrickClip);
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
        /// <param name="other"></param>
        private void CollideWithPlayer(Collision2D other)
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
            Vector2 dir = new Vector2(x, 1).normalized;

            // Set Velocity with dir * speed
            GetComponent<Rigidbody2D>().velocity = dir * _currSpeed;
            if (_audioSource.enabled)
            {
                _audioSource.PlayOneShot(hitPlayerClip);
            }
        }

        /// <summary>
        /// Check for out of bounds
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                DestroyBall();
            }
        }

        /// <summary>
        /// Calculate the horizontal angle of the bounce
        /// </summary>
        /// <param name="ballPos"></param>
        /// <param name="playerPos"></param>
        /// <param name="playerWidth"></param>
        /// <returns></returns>
        private float GetHorizontalVelocityFactor(Vector2 ballPos, Vector2 playerPos,
            float playerWidth)
        {
            float xFactor = (ballPos.x - playerPos.x) / playerWidth;
            Debug.Log($"X factor: {xFactor}");
            return xFactor;
        }

        /// <summary>
        /// Calculate the vertical angle of the bounce
        /// </summary>
        /// <param name="ballPos"></param>
        /// <param name="playerPos"></param>
        /// <param name="playerWidth"></param>
        /// <returns></returns>
        private float GetVerticalVelocityFactor(Vector2 ballPos, Vector2 playerPos, float playerWidth)
        {
            float n = (ballPos.x - playerPos.x) / playerWidth;
            // float yFactor = n < 0 ? n - 1.0f : n + 1.0f;
            float yFactor = 1.0f + Math.Abs(n);
            Debug.Log($"Y factor: {yFactor}");
            // return n < 0 ? n - 1.0f : n + 1.0f;
            return yFactor;
        }

        /// <summary>
        /// Request that the ball be attached to the player
        /// </summary>
        /// <param name="player"></param>
        /// <param name="attachPosition"></param>
        public void Attach(Player player, Vector2 attachPosition)
        {
            if (IsAttached())
            {
                return;
            }
            _attachedPlayer = player;
            LastTouchedByPlayer = player;
            _rb.angularVelocity = 0.0f;
            _rb.velocity = new Vector2(0.0f, 0.0f);
            _rb.isKinematic = true;
            gameObject.transform.SetParent(player.gameObject.transform, true);
            gameObject.transform.position = attachPosition;
            player.AttachBall(this);
            _trailRenderer.enabled = false;
        }

        /// <summary>
        /// Request to detach the ball
        /// </summary>
        /// <param name="player"></param>
        public void Detach(Player player)
        {
            gameObject.transform.SetParent(DefaultParent.transform, true);
            _attachedPlayer = null;
            _rb.isKinematic = false;
            _speedChangeTimer = 0.0f;
            _rb.velocity = (Vector2.up + 0.1f * RandomVector()) * defaultBallSpeed;
            _trailRenderer.enabled = true;
            player.DetachBall(this);
        }

        /// <summary>
        /// Get a random left or right vector
        /// </summary>
        /// <returns></returns>
        private Vector2 RandomVector()
        {
            System.Random rand = new System.Random();
            return rand.Next(0, 100) < 50 ? Vector2.left : Vector2.right;
        }

        /// <summary>
        /// Nudges the ball in the given direction
        /// </summary>
        /// <param name="vectorDirection"></param>
        public void Nudge(Vector2 vectorDirection)
        {
            _rb.velocity = vectorDirection * defaultBallSpeed;
        }

        /// <summary>
        /// Returns the ball to default speed
        /// </summary>
        public void SetDefaultSpeed()
        {
            _currSpeed = defaultBallSpeed;
            _speedChangeTimer = 0.0f;
        }

        /// <summary>
        /// Destroy the ball
        /// </summary>
        private void DestroyBall()
        {
            BallSpeedMultiplierChangeEvent.Invoke(this);
            BallDestroyedEvent.Invoke(this);
        }
    }
}
