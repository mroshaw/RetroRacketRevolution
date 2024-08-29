using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Effects;
using DaftAppleGames.RetroRacketRevolution.Enemies;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Enemy : MonoBehaviour
    {
        [BoxGroup("Movement")] public float accelerationTime = 2f;
        [BoxGroup("Movement")] public float maxSpeed = 5f;

        [BoxGroup("Attack")] public float timeBetweenAttacks = 3.0f;
        [BoxGroup("Attack")] public int percentAttackChance = 20;
        [BoxGroup("Attack")] public Transform bombSpawnTransform;
        [BoxGroup("Attack")] public Sprite projectileSprite;
        [BoxGroup("Attack")] public AudioClip projectileAudioClip;
        [BoxGroup("Attack")] public float projectileScale;
        [BoxGroup("Attack")] public Vector2 colliderSize;

        [BoxGroup("Collision")] public bool canKillPlayer;
        [BoxGroup("Collision")] public bool canBeKilledByPlayer;
        [BoxGroup("Collision")] public bool canBeKilledByOutOfBounds;

        [BoxGroup("Sprites")] public Sprite[] sprites;
        [BoxGroup("Audio")] public AudioClip movingAudioClip;
        [BoxGroup("Settings")] public int score;
        [BoxGroup("Settings")] public int startingHealth = 1;

        [FoldoutGroup("Events")] public UnityEvent OnSpawnEvent;
        [FoldoutGroup("Events")] public UnityEvent<Enemy> EnemyDestroyedEvent;
        [FoldoutGroup("Events")] public UnityEvent<Enemy> EnemyHitEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> EnemyHealthChangedEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> EnemyHealthInitEvent;

        public EnemyManager EnemyManager { get; set; }

        private Vector2 movement;
        private float _moveTimeLeft;
        private float _attackTime;
        private int _health;

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;
        private Flicker _flicker;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public virtual void Awake()
        {
            _moveTimeLeft = accelerationTime;
            _attackTime = 0.0f;
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
            _flicker = GetComponentInChildren<Flicker>();
            _audioSource.clip = movingAudioClip;
            _audioSource.loop = true;

            _health = startingHealth;
        }

        /// <summary>
        /// Initialise other components
        /// </summary>
        public virtual void Start()
        {
            // Notify listeners of starting health
            EnemyHealthInitEvent.Invoke(startingHealth);
        }

        /// <summary>
        /// Move and fire when ready to do so
        /// </summary>
        public virtual void Update()
        {
            _moveTimeLeft -= Time.deltaTime;
            if (_moveTimeLeft <= 0)
            {
                movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                _moveTimeLeft += accelerationTime;
            }
            _attackTime += Time.deltaTime;
            if (_attackTime > timeBetweenAttacks)
            {
                if (DoesWantToAttack())
                {
                    Attack();
                }

                _attackTime = 0.0f;
            }
        }

        /// <summary>
        /// Call this when spawning or retrieving from pool
        /// </summary>
        public virtual void OnSpawn()
        {
            _health = startingHealth;
            _audioSource.Play();
            if (_flicker)
            {
                _flicker.FlickerNow();
            }
            
            OnSpawnEvent.Invoke();
        }

        /// <summary>
        /// Sets a random sprite
        /// </summary>
        public void SetRandomSprite()
        {
            System.Random rnd = new System.Random();
            int spriteIndex = rnd.Next(0, sprites.Length);
            _spriteRenderer.sprite = sprites[spriteIndex];
        }

        /// <summary>
        /// Perform attack
        /// </summary>
        private void Attack()
        {
            // Spawn a projectile instance
            Projectile newProjectile = EnemyManager.SpawnProjectile(projectileSprite, projectileAudioClip, projectileScale);
            newProjectile.gameObject.transform.position = bombSpawnTransform.position;
        }

        /// <summary>
        /// Enemy has been hit by weapon, ball or player
        /// </summary>
        /// <param name="hitByGameObject"></param>
        public void Hit(GameObject hitByGameObject)
        {
            // Hit by player
            Player player = hitByGameObject.GetComponent<Player>();
            if (player)
            {
                // Destroy enemy, if can be killed by player
                if (canBeKilledByPlayer)
                {
                    player.AddScore(score);
                    DestroyEnemy();
                    return;
                }

                // Hit player, if can kill player
                if (canKillPlayer)
                {
                    player.Hit();
                    return;
                }
            }

            // Hit my laser fire
            LaserBolt laserBolt = hitByGameObject.GetComponent<LaserBolt>();
            if (laserBolt)
            {
                _health--;
                EnemyHitEvent.Invoke(this);
                EnemyHealthChangedEvent.Invoke(_health);
                if (_health <= 0)
                {
                    laserBolt.LaserCannon.AttachedPlayer.AddScore(score);
                    DestroyEnemy();
                }

                return;
            }

            // Hit by ball
            Ball ball = hitByGameObject.GetComponent<Ball>();
            if (ball)
            {
                _health--;
                EnemyHitEvent.Invoke(this);
                EnemyHealthChangedEvent.Invoke(_health);
                if (_health <= 0)
                {
                    ball.LastTouchedByPlayer.AddScore(score);
                    DestroyEnemy();
                }
            }
        }

        /// <summary>
        /// Destroys this enemy
        /// </summary>
        public void DestroyEnemy()
        {
            EnemyDestroyedEvent.Invoke(this);
        }

        /// <summary>
        /// Does alien want to fire?
        /// </summary>
        /// <returns></returns>
        private bool DoesWantToAttack()
        {
            System.Random rand = new System.Random();
            return rand.Next(0, 100) < percentAttackChance;
        }

        /// <summary>
        /// Adjust the rigidbody
        /// </summary>
        private void FixedUpdate()
        {
            _rb.AddForce(movement * maxSpeed);
        }

        /// <summary>
        /// Check for out of bounds
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("LogoSprite"))
            {
                EnemyDestroyedEvent.Invoke(this);
                return;
            }

            if (other.gameObject.CompareTag("OutOfBounds") && canBeKilledByOutOfBounds)
            {
                Hit(other.gameObject);
            }
        }

        /// <summary>
        /// Check for collision with players
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
            {
                Hit(other.gameObject);
            }
        }
    }
}
