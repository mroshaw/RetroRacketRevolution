using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Enemies;
using DaftAppleGames.RetroRacketRevolution.Levels;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class EnemyManager : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private GameObject enemyContainer;
        [BoxGroup("Settings")] [SerializeField] private GameObject projectileContainer;

        [BoxGroup("Prefabs")] [SerializeField] private GameObject enemyPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject projectilePrefab;

        [BoxGroup("Bosses")] [SerializeField] private EnemiesData bossEnemies;

        [BoxGroup("Spawn")] [SerializeField] private int maxEnemies;
        [BoxGroup("Spawn")] [SerializeField] private float minTimeBetweenEnemies;
        [BoxGroup("Spawn")] [SerializeField] private float maxTimeBetweenEnemies;
        [BoxGroup("Spawn")] [SerializeField] private float spawnAtY;
        [BoxGroup("Spawn")] [SerializeField] private int minSpawnX;
        [BoxGroup("Spawn")] [SerializeField] private int maxSpawnX;

        [BoxGroup("Debug")] [SerializeField] private float nextSpawnTime;
        [BoxGroup("Debug")] [SerializeField] private float timeSinceLastSpawn;
        [BoxGroup("Debug")] [SerializeField] private List<Enemy> activeEnemies;

        [FoldoutGroup("Events")] public UnityEvent onEnemySpawned;
        [FoldoutGroup("Events")] public UnityEvent onEnemyDestroyed;
        [FoldoutGroup("Events")] public UnityEvent onAllEnemiesPreDestroyed;
        [FoldoutGroup("Events")] public UnityEvent onAllEnemiesDestroyed;

        private ObjectPool<GameObject> _enemyPool;
        private ObjectPool<GameObject> _projectilePool;

        private bool _isSpawning;
        private bool _isBossEnemy;
        private int _bossEnemyIndex;

        /// <summary>
        /// Initialise this component and pools
        /// </summary>
        private void Awake()
        {
            // Initialise the prefab pools
            _enemyPool = new ObjectPool<GameObject>(CreateEnemy, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, 5);
            _projectilePool = new ObjectPool<GameObject>(CreateProjectile, OnTakeProjectileFromPool, OnReturnProjectileToPool, OnDestroyProjectile, true, 10);
            _isSpawning = false;
            timeSinceLastSpawn = 0.0f;
            nextSpawnTime = GetNextSpawnTime();
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
            if (activeEnemies.Count >= maxEnemies || maxEnemies == 0)
            {
                return;
            }
            timeSinceLastSpawn += Time.deltaTime;
            // If we're passed max spawn time, spawn
            if (timeSinceLastSpawn > nextSpawnTime)
            {
                if (_isBossEnemy)
                {
                    SpawnEnemy(true, _bossEnemyIndex);
                }
                else
                {
                    SpawnEnemy(false, 0);
                }

                timeSinceLastSpawn = 0.0f;
                nextSpawnTime = GetNextSpawnTime();
            }
        }

        /// <summary>
        /// Get the next spawn time
        /// </summary>
        private float GetNextSpawnTime()
        {
            double randomTime = Random.Range(minTimeBetweenEnemies, maxTimeBetweenEnemies);
            return (float)randomTime;
        }

        /// <summary>
        /// Starts spawning enemies
        /// </summary>
        [Button("Start Spawning")]
        public void StartSpawning()
        {
            if (maxEnemies > 0)
            {
                nextSpawnTime = GetNextSpawnTime();
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
        [Button("Stop Spawning")]
        public void StopSpawning()
        {
            _isSpawning = false;
        }

        /// <summary>
        /// Set enemy properties from loaded level
        /// </summary>
        public void HandleLevelLoadedEvent(LevelDataExt levelData)
        {
            StopSpawning();
  
            _isBossEnemy = levelData.isBossLevel;
            if (_isBossEnemy)
            {
                _bossEnemyIndex = levelData.levelBossIndex;
                maxEnemies = 1;
                minTimeBetweenEnemies = 0;
                maxTimeBetweenEnemies = 0;
            }
            else
            {
                maxEnemies = levelData.maxEnemies;
                minTimeBetweenEnemies = levelData.minTimeBetweenEnemies;
                maxTimeBetweenEnemies = levelData.maxTimeBetweenEnemies;
                StartSpawning();
            }
        }

        /// <summary>
        /// Destroys all active enemies
        /// </summary>
        public void RemoveAllEnemies()
        {
            foreach (Enemy enemy in activeEnemies.ToArray())
            {
                enemy.Die();
            }
        }

        /// <summary>
        /// Spawn an Enemy from the Pool
        /// </summary>
        private Enemy SpawnEnemy(bool isBossEnemy, int bossEnemyIndex)
        {
            if (isBossEnemy)
            {
                System.Random rand = new System.Random();
                float horizontal = rand.Next(minSpawnX, maxSpawnX);
                GameObject enemyGameObject = Instantiate(bossEnemies.EnemyList[bossEnemyIndex].enemyPrefab);
                enemyGameObject.transform.SetParent(enemyContainer.transform);
                Vector3 newPosition = new(horizontal, spawnAtY, 0.0f);
                enemyGameObject.transform.position = newPosition;
                Enemy newEnemy = enemyGameObject.GetComponent<Enemy>();
                newEnemy.EnemyManager = this;
                newEnemy.onDestroyed.AddListener(EnemyDestroyed);
                newEnemy.OnSpawn();
                activeEnemies.Add(newEnemy);
                onEnemySpawned.Invoke();
                return newEnemy;
            }
            else
            {
                float horizontal = Random.Range(minSpawnX, maxSpawnX);

                GameObject enemyGameObject = _enemyPool.Get();
                Vector3 newPosition = new(horizontal, spawnAtY, 0);
                enemyGameObject.transform.position = newPosition;
                Enemy newEnemy = enemyGameObject.GetComponent<Enemy>();
                newEnemy.OnSpawn();
                onEnemySpawned.Invoke();
                return newEnemy;
            }
        }

        /// <summary>
        /// Spawn a Projectile from the Pool
        /// </summary>
        private EnemyProjectile SpawnProjectile(float projectileScale)
        {
            GameObject projectileGameObject = _projectilePool.Get();
            EnemyProjectile newProjectile = projectileGameObject.GetComponent<EnemyProjectile>();
            newProjectile.gameObject.transform.localScale = new Vector2(projectileScale, projectileScale);
            newProjectile.InitProjectile(projectileScale);
            return newProjectile;
        }

        /// <summary>
        /// Call back for the Projectile destroyed event
        /// </summary>
        private void ProjectileDestroyed(Projectile projectile)
        {
            _projectilePool.Release(projectile.gameObject);
        }

        /// <summary>
        /// Call back for the Enemy destroyed event
        /// </summary>
        private void EnemyDestroyed(Enemy enemy)
        {
            if (_isBossEnemy)
            {
                activeEnemies.Remove(enemy);
                Destroy(enemy.gameObject);
            }
            else
            {
                _enemyPool.Release(enemy.gameObject);
            }
            onEnemyDestroyed.Invoke();
            // Check if this is the last enemy destroyed. Used mainly for Boss enemies
            if (activeEnemies.Count == 0)
            {
                if (_isBossEnemy)
                {
                    StopSpawning();
                    onAllEnemiesPreDestroyed.Invoke();
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
            onAllEnemiesDestroyed.Invoke();
        }

        #region PoolRegion
        /// <summary>
        /// Create action for pool
        /// </summary>
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
        private void OnTakeEnemyFromPool(GameObject enemy)
        {
            enemy.SetActive(true);
            enemy.GetComponent<Enemy>().Reset();
            enemy.GetComponent<Enemy>().onDestroyed.AddListener(EnemyDestroyed);
            activeEnemies.Add(enemy.GetComponent<Enemy>());
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnEnemyToPool(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().onDestroyed.RemoveListener(EnemyDestroyed);
            enemy.SetActive(false);
            activeEnemies.Remove(enemy.GetComponent<Enemy>());
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyEnemy(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().onDestroyed.RemoveListener(EnemyDestroyed);
            activeEnemies.Remove(enemy.GetComponent<Enemy>());
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
            EnemyProjectile projectile = projectileGameObject.GetComponent<EnemyProjectile>();
            projectile.EnemyManager = this;
            return projectileGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        private void OnTakeProjectileFromPool(GameObject projectile)
        {
            projectile.SetActive(true);
            projectile.GetComponent<EnemyProjectile>().ProjectileDestroyedEvent.AddListener(ProjectileDestroyed);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnProjectileToPool(GameObject projectile)
        {
            projectile.GetComponent<EnemyProjectile>().ProjectileDestroyedEvent.RemoveListener(ProjectileDestroyed);
            projectile.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyProjectile(GameObject projectile)
        {
            projectile.GetComponent<EnemyProjectile>().ProjectileDestroyedEvent.RemoveListener(ProjectileDestroyed);
            Destroy(projectile);
        }
        #endregion
    }
}