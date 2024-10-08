using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Enemies;
using DaftAppleGames.RetroRacketRevolution.Levels;
using UnityEngine;
using UnityEngine.Events;


namespace DaftAppleGames.RetroRacketRevolution
{
    public class EnemyManager : MonoBehaviour
    {
        [BoxGroup("Settings")] public GameObject enemyContainer;
        [BoxGroup("Settings")] public GameObject projectileContainer;
        [BoxGroup("Settings")] public GameObject explosionContainer;

        [BoxGroup("Prefabs")] public GameObject enemyPrefab;
        [BoxGroup("Prefabs")] public GameObject projectilePrefab;
        [BoxGroup("Prefabs")] public GameObject explosionPrefab;

        [BoxGroup("Bosses")] public EnemiesData bossEnemies;

        [BoxGroup("Spawn")] public float spawnAtY;
        [BoxGroup("Spawn")] public int minSpawnX;
        [BoxGroup("Spawn")] public int maxSpawnX;
        [SerializeField]
        [BoxGroup("Spawn")] private List<Enemy> _activeEnemies;

        [BoxGroup("Pool")] public UnityEngine.Pool.ObjectPool<GameObject> enemyPool;
        [BoxGroup("Pool")] public UnityEngine.Pool.ObjectPool<GameObject> projectilePool;
        [BoxGroup("Pool")] public UnityEngine.Pool.ObjectPool<GameObject> explosionPool;

        private bool _isSpawning;
        private bool _isBossEnemy = false;
        private int _bossEnemyIndex;

        private System.Random _rand = new System.Random();
        private AudioSource _audioSource;

        [BoxGroup("Debug")] [SerializeField] private float _nextSpawnTime;
        [BoxGroup("Debug")] [SerializeField] private float _timeSinceLastSpawn;
        [BoxGroup("Debug")] [SerializeField] private int _maxEnemies;
        [BoxGroup("Debug")] [SerializeField] private float _minTimeBetweenEnemies;
        [BoxGroup("Debug")] [SerializeField] private float _maxTimeBetweenEnemies;

        [FoldoutGroup("Events")] public UnityEvent EnemySpawnedEvent;
        [FoldoutGroup("Events")] public UnityEvent EnemyDestroyedEvent;
        [FoldoutGroup("Events")] public UnityEvent AllEnemiesDestroyedPreEvent;
        [FoldoutGroup("Events")] public UnityEvent AllEnemiesDestroyedEvent;

        /// <summary>
        /// Initialise this component and pools
        /// </summary>
        private void Awake()
        {
            // Initialise the prefab pools
            enemyPool = new UnityEngine.Pool.ObjectPool<GameObject>(CreateEnemy, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 5);
            projectilePool = new UnityEngine.Pool.ObjectPool<GameObject>(CreateProjectile, OnTakeProjectileFromPool, OnReturnProjectileToPool, OnDestroyProjectile, true, 10);
            explosionPool = new UnityEngine.Pool.ObjectPool<GameObject>(CreateExplosion, OnTakeExplosionFromPool, OnReturnExplosionToPool, OnDestroyExplosion, true, 10);
            _isSpawning = false;
            _timeSinceLastSpawn = 0.0f;
            _nextSpawnTime = GetNextSpawnTime();
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Setup other components
        /// </summary>
        private void Start()
        {
        
        }

        /// <summary>
        /// Manage enemy spawning
        /// </summary>
        private void Update()
        {
            // Don't do anything until we're actually spawning
            if (!_isSpawning)
            {
                return;
            }

            // Do nothing if we've already got max enemies
            if (_activeEnemies.Count >= _maxEnemies || _maxEnemies == 0)
            {
                return;
            }
            _timeSinceLastSpawn += Time.deltaTime;
            // If we're passed max spawn time, spawn
            if (_timeSinceLastSpawn > _nextSpawnTime)
            {
                if (_isBossEnemy)
                {
                    SpawnEnemy(true, _bossEnemyIndex);
                }
                else
                {
                    SpawnEnemy(false, 0);
                }

                _timeSinceLastSpawn = 0.0f;
                _nextSpawnTime = GetNextSpawnTime();
                return;
            }
        }

        /// <summary>
        /// Get the next spawn time
        /// </summary>
        /// <returns></returns>
        private float GetNextSpawnTime()
        {
            double randomTime = _rand.NextDouble() * (_maxTimeBetweenEnemies - _minTimeBetweenEnemies) + _minTimeBetweenEnemies;
            return (float)randomTime;
        }

        /// <summary>
        /// Starts spawning enemies
        /// </summary>
        public void StartSpawning()
        {
            if (_maxEnemies > 0)
            {
                _nextSpawnTime = GetNextSpawnTime();
                _isSpawning = true;
            }
            else
            {
                _isSpawning = false;
            }
        }

        /// <summary>
        /// Stop spawning
        /// </summary>
        public void StopSpawning()
        {
            _isSpawning = false;
        }

        /// <summary>
        /// Set enemy properties from loaded level
        /// </summary>
        /// <param name="levelData"></param>
        public void HandleLevelLoadedEvent(LevelDataExt levelData)
        {
            StopSpawning();
  
            _isBossEnemy = levelData.isBossLevel;
            if (_isBossEnemy)
            {
                _bossEnemyIndex = levelData.levelBossIndex;
                _maxEnemies = 1;
                _minTimeBetweenEnemies = 0;
                _maxTimeBetweenEnemies = 0;
            }
            else
            {
                _maxEnemies = levelData.maxEnemies;
                _minTimeBetweenEnemies = levelData.minTimeBetweenEnemies;
                _maxTimeBetweenEnemies = levelData.maxTimeBetweenEnemies;
                StartSpawning();
            }
        }

        /// <summary>
        /// Destroys all active enemies
        /// </summary>
        public void RemoveAllEnemies()
        {
            foreach (Enemy enemy in _activeEnemies.ToArray())
            {
                enemy.DestroyEnemy();
            }
        }

        /// <summary>
        /// Spawn an Enemy from the Pool
        /// </summary>
        /// <returns></returns>
        public Enemy SpawnEnemy(bool isBossEnemy, int bossEnemyIndex)
        {
            if (isBossEnemy)
            {
                System.Random rand = new System.Random();
                float horizontal = rand.Next(minSpawnX, maxSpawnX);
                GameObject enemyGameObject = Instantiate(bossEnemies.EnemyList[bossEnemyIndex].enemyPrefab);
                enemyGameObject.transform.SetParent(enemyContainer.transform);
                Vector2 newPosition = new Vector2(horizontal, spawnAtY);
                enemyGameObject.transform.position = newPosition;
                Enemy newEnemy = enemyGameObject.GetComponent<Enemy>();
                newEnemy.EnemyManager = this;
                newEnemy.EnemyDestroyedEvent.AddListener(EnemyDestroyed);
                newEnemy.OnSpawn();
                _activeEnemies.Add(newEnemy);
                EnemySpawnedEvent.Invoke();
                return newEnemy;
            }
            else
            {
                System.Random rand = new System.Random();
                float horizontal = rand.Next(minSpawnX, maxSpawnX);

                GameObject enemyGameObject = enemyPool.Get();
                Vector2 newPosition = new Vector2(horizontal, spawnAtY);
                enemyGameObject.transform.position = newPosition;
                Enemy newEnemy = enemyGameObject.GetComponent<Enemy>();
                newEnemy.OnSpawn();
                EnemySpawnedEvent.Invoke();
                return newEnemy;
            }
        }

        /// <summary>
        /// Spawn a Projectile from the Pool
        /// </summary>
        /// <returns></returns>
        public Projectile SpawnProjectile(Sprite projectileSprite, AudioClip fallingAudioClip, float projectileScale)
        {
            GameObject projectileGameObject = projectilePool.Get();
            Projectile newProjectile = projectileGameObject.GetComponent<Projectile>();
            newProjectile.sprite = projectileSprite;
            newProjectile.fallingAudioClip = fallingAudioClip;
            newProjectile.gameObject.transform.localScale = new Vector2(projectileScale, projectileScale);
            newProjectile.InitProjectile(projectileSprite, fallingAudioClip, projectileScale);
            return newProjectile;
        }

        /// <summary>
        /// Call back for the Projectile destroyed event
        /// </summary>
        /// <param name="projectile"></param>
        public void ProjectileDestroyed(Projectile projectile)
        {
            projectilePool.Release(projectile.gameObject);
        }

        /// <summary>
        /// Call back for the Enemy destroyed event
        /// </summary>
        /// <param name="enemy"></param>
        public void EnemyDestroyed(Enemy enemy)
        {
            // Spawn explosion
            GameObject explosion = explosionPool.Get();
            explosion.transform.position = enemy.gameObject.transform.position;
            explosion.GetComponent<Explosion>().Explode(true);
            if (_isBossEnemy)
            {
                _activeEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }
            else
            {
                enemyPool.Release(enemy.gameObject);
            }
            EnemyDestroyedEvent.Invoke();
            // Check if this is the last enemy destroyed. Used mainly for Boss enemies
            if (_activeEnemies.Count == 0)
            {
                if (_isBossEnemy)
                {
                    StopSpawning();
                    AllEnemiesDestroyedPreEvent.Invoke();
                    StartCoroutine(WaitAndAnnounceLastEnemyDestroyed());
                }
            }
        }

        /// <summary>
        /// Waits for 2 seconds then reports last enemy destroyed. Allows destruction animation to play
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndAnnounceLastEnemyDestroyed()
        {
            yield return new WaitForSeconds(2);
            AllEnemiesDestroyedEvent.Invoke();
        }

        #region PoolRegion
        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateEnemy()
        {
            GameObject enemyGameObject = Instantiate(enemyPrefab);
            enemyGameObject.transform.parent = enemyContainer.transform;
            Enemy enemy = enemyGameObject.GetComponent<Enemy>();
            enemy.EnemyManager = this;
            return enemyGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="enemy"></param>
        private void OnTakeEnemyFromPool(GameObject enemy)
        {
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().EnemyDestroyedEvent.AddListener(EnemyDestroyed);
            _activeEnemies.Add(enemy.GetComponent<Enemy>());
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        /// <param name="enemy"></param>
        private void OnReturnEnemyToPool(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().EnemyDestroyedEvent.RemoveListener(EnemyDestroyed);
            enemy.SetActive(false);
            _activeEnemies.Remove(enemy.GetComponent<Enemy>());
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        /// <param name="enemy"></param>
        private void OnDestroyEnemy(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().EnemyDestroyedEvent.RemoveListener(EnemyDestroyed);
            _activeEnemies.Remove(enemy.GetComponent<Enemy>());
            Destroy(enemy);
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateProjectile()
        {
            GameObject projectileGameObject = Instantiate(projectilePrefab);
            projectileGameObject.transform.parent = projectileContainer.transform;
            Projectile projectile = projectileGameObject.GetComponent<Projectile>();
            projectile.EnemyManager = this;
            return projectileGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="projectile"></param>
        private void OnTakeProjectileFromPool(GameObject projectile)
        {
            projectile.SetActive(true);
            projectile.GetComponent<Projectile>().ProjectileDestroyedEvent.AddListener(ProjectileDestroyed);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        /// <param name="projectile"></param>
        private void OnReturnProjectileToPool(GameObject projectile)
        {
            projectile.GetComponent<Projectile>().ProjectileDestroyedEvent.RemoveListener(ProjectileDestroyed);
            projectile.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        /// <param name="projectile"></param>
        private void OnDestroyProjectile(GameObject projectile)
        {
            projectile.GetComponent<Projectile>().ProjectileDestroyedEvent.RemoveListener(ProjectileDestroyed);
            Destroy(projectile);
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateExplosion()
        {
            GameObject explosionGameObject = Instantiate(explosionPrefab);
            explosionGameObject.transform.SetParent(explosionContainer.transform);
            Explosion explosion = explosionGameObject.GetComponent<Explosion>();
            explosion.ReturnToPoolEvent.AddListener(ReturnExplosionToPool);
            return explosionGameObject;
        }

        /// <summary>
        /// Release an explosion back to the pool
        /// </summary>
        /// <param name="explosion"></param>
        private void ReturnExplosionToPool(Explosion explosion)
        {
            explosionPool.Release(explosion.gameObject);
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="explosion"></param>
        private void OnTakeExplosionFromPool(GameObject explosion)
        {
            explosion.SetActive(true);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        /// <param name="explosion"></param>
        private void OnReturnExplosionToPool(GameObject explosion)
        {
            explosion.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        /// <param name="explosion"></param>
        private void OnDestroyExplosion(GameObject explosion)
        {
            Destroy(explosion);
        }


        #endregion
    }
}
