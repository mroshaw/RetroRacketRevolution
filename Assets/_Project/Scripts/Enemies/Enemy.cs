using System.Collections;
using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Balls;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class Enemy : MonoBehaviour
    {
        [BoxGroup("Movement")] public float accelerationTime = 2f;
        [BoxGroup("Movement")] public float maxSpeed = 5f;
        [BoxGroup("Attack")] public float timeBetweenAttacks = 3.0f;
        [BoxGroup("Attack")] public int percentAttackChance = 20;
        [BoxGroup("Attack")] public Transform bombSpawnTransform;
        [BoxGroup("Sprites")] public Sprite[] sprites;
        [BoxGroup("Audio")] public AudioClip movingAudioClip;
        [BoxGroup("Settings")] public int score;

        [FoldoutGroup("Events")] public UnityEvent<Enemy> EnemyDestroyedEvent;

        public EnemyManager EnemyManager { get; set; }

        private Vector2 movement;
        private float _moveTimeLeft;
        private float _attackTime;

        private System.Random rnd = new System.Random();

        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private AudioSource _audioSource;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _moveTimeLeft = accelerationTime;
            _attackTime = 0.0f;
            _rb = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Initialise other components
        /// </summary>
        private void Start()
        {
            _audioSource.clip = movingAudioClip;
            _audioSource.loop = true;
            _audioSource.Play();
        }

        /// <summary>
        /// Move and fire when ready to do so
        /// </summary>
        void Update()
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
        /// Sets a random sprite
        /// </summary>
        public void SetRandomSprite()
        {
            int spriteIndex = rnd.Next(0, sprites.Length);
            _spriteRenderer.sprite = sprites[spriteIndex];
        }

        /// <summary>
        /// Perform attack
        /// </summary>
        private void Attack()
        {
            // Spawn a projectile instance
            Projectile newProjectile = EnemyManager.SpawnProjectile();
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
                player.AddScore(score);
            }

            // Hit by ball
            Ball ball = hitByGameObject.GetComponent<Ball>();
            if (ball)
            {
                ball.LastTouchedByPlayer.AddScore(score);
            }

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
            if (other.gameObject.CompareTag("OutOfBounds"))
            {
                EnemyDestroyedEvent.Invoke(this);
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
                EnemyDestroyedEvent.Invoke(this);
            }
        }
    }
}
