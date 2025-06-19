using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Enemy : MonoBehaviour
    {
        [BoxGroup("Movement")] [SerializeField] private float accelerationTime = 2f;
        [BoxGroup("Movement")] [SerializeField] private float maxSpeed = 5f;
        [BoxGroup("Movement")] [SerializeField] private AudioClip movingAudioClip;

        [BoxGroup("Attack")] [SerializeField] private float timeBetweenAttacks = 3.0f;
        [BoxGroup("Attack")] [SerializeField] private int percentAttackChance = 20;
        [BoxGroup("Attack")] [SerializeField] private Transform bombSpawnTransform;
        [BoxGroup("Attack")] [SerializeField] private AudioClip attackAudioClip;

        [BoxGroup("Collision")] [SerializeField] private bool canKillPlayer;
        [BoxGroup("Collision")] [SerializeField] private bool canBeKilledByPlayer;
        [BoxGroup("Collision")] [SerializeField] private bool canBeKilledByOutOfBounds;

        [BoxGroup("Settings")] [SerializeField] private Transform model;
        [BoxGroup("Settings")] [SerializeField] private int score;
        [BoxGroup("Settings")] [SerializeField] private int startingHealth = 1;

        [FoldoutGroup("Events")] public UnityEvent onSpawn;
        [FoldoutGroup("Events")] public UnityEvent<Enemy> onDestroyed;
        [FoldoutGroup("Events")] public UnityEvent<Enemy> onHit;
        [FoldoutGroup("Events")] public UnityEvent<int> onHealthChanged;
        [FoldoutGroup("Events")] public UnityEvent<int> onHealthInit;

        public EnemyManager EnemyManager { get; set; }

        private Vector3 _movement;
        private float _moveTimeLeft;
        private float _attackTime;
        private int _health;

        private Rigidbody _rb;
        private AudioSource _audioSource;
        private Explosion _explosion;

        private Collider _collider;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public virtual void Awake()
        {
            _moveTimeLeft = accelerationTime;
            _attackTime = 0.0f;
            _rb = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = movingAudioClip;
            _audioSource.loop = true;
            _health = startingHealth;
            _explosion = GetComponentInChildren<Explosion>();
            _collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Initialise other components
        /// </summary>
        internal virtual void Start()
        {
            // Notify listeners of starting health
            onHealthInit.Invoke(startingHealth);
        }

        /// <summary>
        /// Move and fire when ready to do so
        /// </summary>
        internal virtual void Update()
        {
            _moveTimeLeft -= Time.deltaTime;
            if (_moveTimeLeft <= 0)
            {
                _movement = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
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
            onSpawn.Invoke();
        }

        /// <summary>
        /// Perform attack
        /// </summary>
        private void Attack()
        {
            // Spawn a projectile instance
            /*
            EnemyProjectile newProjectile = EnemyManager.SpawnProjectile(projectileSprite, attackAudioClip, projectileScale);
            newProjectile.gameObject.transform.position = bombSpawnTransform.position;
            */
        }

        /// <summary>
        /// Enemy has been hit by weapon, ball or player
        /// </summary>
        public void Hit(GameObject hitByGameObject)
        {
            // Hit by boundary
            if(hitByGameObject.layer == LayerMask.NameToLayer("OutOfBounds"))
            {
                Die();
                return;
            }

            // Hit by player
            Player player = hitByGameObject.GetComponent<Player>();
            if (player)
            {
                // Destroy enemy, if can be killed by player
                if (canBeKilledByPlayer)
                {
                    player.AddScore(score);
                    Die();
                    return;
                }

                // Hit player, if can kill player
                if (canKillPlayer)
                {
                    player.Hit();
                    return;
                }
            }

            // Hit by projectile
            Projectile projectile = hitByGameObject.GetComponent<Projectile>();
            if (projectile)
            {
                _health--;
                onHit.Invoke(this);
                onHealthChanged.Invoke(_health);
                if (_health <= 0)
                {
                    projectile.WeaponAddOn.AttachedPlayer.AddScore(score);
                    Die();
                }

                return;
            }

            // Hit by ball
            Ball ball = hitByGameObject.GetComponent<Ball>();
            if (ball)
            {
                _health--;
                onHit.Invoke(this);
                onHealthChanged.Invoke(_health);
                if (_health <= 0)
                {
                    ball.LastTouchedByPlayer.AddScore(score);
                    Die();
                }
            }
        }

        /// <summary>
        /// Destroys this enemy
        /// </summary>
        public void Die()
        {
            model.gameObject.SetActive(false);
            _collider.enabled = false;
            _explosion.Explode(true);
            Invoke(nameof(DieAfterDelay), 5.0f);
        }

        private void DieAfterDelay()
        {
            _explosion.ResetExplosion();
            gameObject.SetActive(false);
            onDestroyed.Invoke(this);
        }

        /// <summary>
        /// Reset for re-use in pool
        /// </summary>
        internal void Reset()
        {
            model.gameObject.SetActive(true);
            _collider.enabled = true;
        }

        /// <summary>
        /// Does alien want to fire?
        /// </summary>
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
            _rb.AddForce(_movement * maxSpeed);
        }

        /// <summary>
        /// Check for out of bounds
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("LogoSprite"))
            {
                onDestroyed.Invoke(this);
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
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Player2"))
            {
                Hit(other.gameObject);
            }
        }
    }
}