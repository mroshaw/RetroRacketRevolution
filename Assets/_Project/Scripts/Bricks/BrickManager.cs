using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    public class BrickManager : MonoBehaviour
    {
        [BoxGroup("Audio")] [SerializeField] private AudioClip destroyedClip;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject disruptorPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickContainer;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject disruptorContainer;

        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickExplosionPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickExplosionContainer;

        [BoxGroup("Prefabs")] [SerializeField] private bool usePooling=false;
        [BoxGroup("Prefabs")] [SerializeField] private int defaultPoolSize = 200;
        [BoxGroup("Debug")] [SerializeField] private List<Brick> bricks = new List<Brick>();
        [BoxGroup("Debug")] [SerializeField] private List<Disruptor> disruptors = new List<Disruptor>();

        [BoxGroup("Events")] public UnityEvent<Brick> BrickAddedEvent;
        [BoxGroup("Events")] public UnityEvent<Brick> BrickDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent<Transform> BrickDestroyedAtPositionEvent;
        [BoxGroup("Events")] public UnityEvent LastBrickDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent<BonusType, Vector2> BrickSpawnBonusEvent;

        // Private properties
        private AudioSource _audioSource;

        private int _destructableBricks;

        // Brick prefab pool
        [BoxGroup("Pool")] public ObjectPool<GameObject> brickPool;
        [BoxGroup("Pool")] public ObjectPool<GameObject> brickExplosionPool;
        /// <summary>
        /// Set up the Brick Manager
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (usePooling)
            {
                brickPool = new ObjectPool<GameObject>(CreateBrick, OnTakeBrickFromPool, OnReturnBrickToPool, OnDestroyBrick, true, defaultPoolSize);
                brickExplosionPool = new ObjectPool<GameObject>(CreateBrickExplosion, OnTakeBrickExplosionFromPool,
                    OnReturnBrickExplosionToPool, OnDestroyBrickExplosion, true, defaultPoolSize);
            }
        }

        /// <summary>
        /// Simulate player destroying all destructable bricks
        /// </summary>
        [BoxGroup("Debug")]
        [Button("Destroy All Bricks")]
        internal void DestroyAllBricks()
        {
            foreach (Brick brick in bricks.ToArray())
            {
                if (brick.BrickType != BrickType.Invincible)
                {
                    DestroyBrickCallBack(brick, false);
                }
            }
        }

        /// <summary>
        /// Spawn a new brick with the given properties
        /// </summary>
        internal Brick SpawnBrick(BrickType brickType, Color brickColor, BonusType brickBonus, int row, int col,
            bool isMainSortingGroup)
        {
            GameObject newBrickGameObject;
            if (usePooling)
            {
                newBrickGameObject = brickPool.Get();
            }
            else
            {
                newBrickGameObject = Instantiate(brickPrefab);
                newBrickGameObject.GetComponent<Brick>().BrickDestroyedEvent.AddListener(DestroyBrick);
            }

            Brick brick = newBrickGameObject.GetComponent<Brick>();
            bricks.Add(brick);
            brick.BrickManager = this;
            brick.transform.SetParent(brickContainer.transform);
            brick.ReConfigureBrick(brickType, brickColor, brickBonus);
            brick.SetSortingGroup(isMainSortingGroup);
            if (brickType != BrickType.Invincible)
            {
                _destructableBricks++;
            }
            brick.OnSpawn();
            return brick;
        }

        /// <summary>
        /// Destroyed brick has a bonus to spawn
        /// </summary>
        public void BrickSpawnBonus(BonusType bonusType, Vector2 spawnPosition)
        {
            BrickSpawnBonusEvent.Invoke(bonusType, spawnPosition);
        }

        /// <summary>
        /// Spawns a Disruptor
        /// </summary>
        public Disruptor SpawnDisruptor(BrickType brickType, int row, int col)
        {
            GameObject newDisruptorGameObject = Instantiate(disruptorPrefab, disruptorContainer.transform);
            Disruptor newDisruptor = newDisruptorGameObject.GetComponent<Disruptor>();
            switch (brickType)
            {
                case BrickType.DisruptorIn:
                    newDisruptor.direction = VortexDirection.Inward;
                    break;
                case BrickType.DisruptorOut:
                    newDisruptor.direction = VortexDirection.Outward;
                    break;
                case BrickType.DisruptorBoth:
                    newDisruptor.direction = VortexDirection.Both;
                    break;
            }

            disruptors.Add(newDisruptor);

            return newDisruptor;
        }

        /// <summary>
        /// Destroys all disruptors
        /// </summary>
        internal void DestroyAllDisruptors()
        {
            foreach (Disruptor currDisruptor in disruptors.ToArray())
            {
                DestroyDisruptor(currDisruptor);
            }
        }

        /// <summary>
        /// Destroys the given disruptor
        /// </summary>
        private void DestroyDisruptor(Disruptor disruptor)
        {
            if (disruptor != null)
            {
                Destroy(disruptor.gameObject);
            }
        }

        /// <summary>
        /// Callback from Brick destroyed event
        /// </summary>
        private void DestroyBrickCallBack(Brick brick, bool playSound)
        {
            DestroyBrick(brick, playSound);

            // Check if this is the last destructable brick
            if (HasLastBrickBeenDestroyed())
            {
                // Clear down the indestructable bricks
                ClearRemainingBricks();

                // Call event listeners
                LastBrickDestroyedEvent.Invoke();
            }
        }

        /// <summary>
        /// Destroy a brick
        /// </summary>
        private void DestroyBrick(Brick brick, bool playSound)
        {
            if (brick.BrickType == BrickType.Invincible)
            {
                playSound = false;
            }
            bricks.Remove(brick);
            if (brick.BrickType != BrickType.Invincible)
            {
                _destructableBricks--;
            }

            BrickDestroyedAtPositionEvent.Invoke(brick.transform);

            if (usePooling)
            {
                GameObject brickExplosionGameObject = brickExplosionPool.Get();
                brickExplosionGameObject.transform.position = brick.transform.position;
                brickExplosionGameObject.GetComponent<Explosion>().Explode(playSound);
                brickPool.Release(brick.gameObject);
            }
            else
            {
                Destroy(brick.gameObject);
            }
        }

        /// <summary>
        /// Clear down any bricks in the list
        /// </summary>
        private void ClearRemainingBricks()
        {
            foreach (Brick brick in bricks.ToArray())
            {
                DestroyBrick(brick, false);
            }
        }

        /// <summary>
        /// Are there any destructible bricks left?
        /// </summary>
        private bool HasLastBrickBeenDestroyed()
        {
            foreach (Brick brick in bricks)
            {
                if (brick.BrickType != BrickType.Invincible)
                {
                    return false;
                }
            }
            return true;
        }

        #region PoolRegion
        /// <summary>
        /// Create action for pool
        /// </summary>
        private GameObject CreateBrick()
        {
            GameObject newBrickGameObject = Instantiate(brickPrefab, brickContainer.transform, true);
            Brick brick = newBrickGameObject.GetComponent<Brick>();
            brick.BrickManager = this;
            // Debug.Log($"Created brick in pool. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
            return newBrickGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        private void OnTakeBrickFromPool(GameObject brickGameObject)
        {
            Brick brick = brickGameObject.GetComponent<Brick>();
            brickGameObject.SetActive(true);
            brick.BrickDestroyedEvent.AddListener(DestroyBrickCallBack);
            // Debug.Log($"Brick retrieved from pool. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnBrickToPool(GameObject brickGameObject)
        {
            Brick brick = brickGameObject.GetComponent<Brick>();
            brick.BrickDestroyedEvent.RemoveAllListeners();
            brickGameObject.SetActive(false);
            // Debug.Log($"Brick returned to pool: {brick.brickType}, {brick.brickColor}. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyBrick(GameObject brickGameObject)
        {
            Brick brick = brickGameObject.GetComponent<Brick>();
            // Debug.Log($"Brick destroyed to pool. {brick.brickType}, {brick.brickColor}. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
            Destroy(brickGameObject);
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        private GameObject CreateBrickExplosion()
        {
            GameObject newBrickExplosionGameObject = Instantiate(brickExplosionPrefab);
            newBrickExplosionGameObject.transform.parent = brickExplosionContainer.transform;
            Explosion explosion = newBrickExplosionGameObject.GetComponent<Explosion>();
            explosion.ReturnToPoolEvent.AddListener(ReturnBrickExplosionToPool);
            return newBrickExplosionGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        private void OnTakeBrickExplosionFromPool(GameObject brickExplosionGameObject)
        {
            brickExplosionGameObject.SetActive(true);
        }

        /// <summary>
        /// Release an explosion back to the pool
        /// </summary>
        private void ReturnBrickExplosionToPool(Explosion explosion)
        {
            explosion.ResetExplosion();
            brickExplosionPool.Release(explosion.gameObject);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnBrickExplosionToPool(GameObject brickExplosionGameObject)
        {
            brickExplosionGameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyBrickExplosion(GameObject brickExplosionGameObject)
        {
            Destroy(brickExplosionGameObject);
        }
        #endregion
    }
}