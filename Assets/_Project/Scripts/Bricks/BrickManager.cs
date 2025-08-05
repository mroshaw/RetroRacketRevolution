using System.Collections;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    public class BrickManager : MonoBehaviour
    {
        [BoxGroup("Audio")] [SerializeField] private AudioClip destroyedClip;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject disruptorPrefab;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject brickContainer;
        [BoxGroup("Prefabs")] [SerializeField] private GameObject disruptorContainer;
        [BoxGroup("Prefabs")] [SerializeField] private float delayBeforePoolReturn = 3.0f;
        [BoxGroup("Prefabs")] [SerializeField] private int defaultPoolSize = 200;
        [BoxGroup("Debug")] [SerializeField] private List<Brick> bricks = new List<Brick>();
        [BoxGroup("Debug")] [SerializeField] private List<Disruptor> disruptors = new List<Disruptor>();

        [FormerlySerializedAs("BrickAddedEvent")] [BoxGroup("Events")] public UnityEvent<Brick> onBrickAdded;
        [BoxGroup("Events")] public UnityEvent<Brick> onBrickDestroyed;
        [BoxGroup("Events")] public UnityEvent onLastBrickDestroyed;
        [BoxGroup("Events")] public UnityEvent<BonusType, Vector3> onBrickBonusSpawned;

        // Brick prefab pool
        private ObjectPool<GameObject> _brickPool;

        /// <summary>
        /// Set up the Brick Manager
        /// </summary>
        private void Awake()
        {
            _brickPool = new ObjectPool<GameObject>(CreateBrick, OnTakeBrickFromPool, OnReturnBrickToPool,
                OnDestroyBrick, true, defaultPoolSize);
        }

        /// <summary>
        /// Simulate player destroying all destructable bricks
        /// </summary>
        [BoxGroup("Debug")]
        [Button("Destroy All Bricks")]
        public void DestroyAllBricks()
        {
            foreach (Brick brick in bricks.ToArray())
            {
                if (brick.BrickType != BrickType.Invincible)
                {
                    DestroyBrickCallBack(brick);
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

            newBrickGameObject = _brickPool.Get();

            Brick brick = newBrickGameObject.GetComponent<Brick>();
            bricks.Add(brick);
            brick.BrickManager = this;
            brick.transform.SetParent(brickContainer.transform);
            brick.ReConfigureBrick(brickType, brickColor, brickBonus);
            brick.SetSortingGroup(isMainSortingGroup);
            brick.OnSpawn();
            return brick;
        }

        /// <summary>
        /// Destroyed brick has a bonus to spawn
        /// </summary>
        public void BrickSpawnBonus(BonusType bonusType, Vector3 spawnPosition)
        {
            onBrickBonusSpawned.Invoke(bonusType, spawnPosition);
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
        private void DestroyBrickCallBack(Brick brick)
        {
            DestroyBrick(brick);

            // Check if this is the last destructable brick
            if (HasLastBrickBeenDestroyed())
            {
                // Clear down the indestructable bricks and disruptors
                ClearRemainingBricks();
                DestroyAllDisruptors();

                // Call event listeners
                onLastBrickDestroyed.Invoke();
            }
        }

        /// <summary>
        /// Destroy a brick
        /// </summary>
        private void DestroyBrick(Brick brick)
        {
            bricks.Remove(brick);
            StartCoroutine(DestroyBrickAfterDelay(brick));
        }

        private IEnumerator DestroyBrickAfterDelay(Brick brick)
        {
            yield return new WaitForSeconds(delayBeforePoolReturn);
            _brickPool.Release(brick.gameObject);
        }

        /// <summary>
        /// Clear down any bricks in the list
        /// </summary>
        private void ClearRemainingBricks()
        {
            foreach (Brick brick in bricks.ToArray())
            {
                DestroyBrick(brick);
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
            brick.onDestroyed.AddListener(DestroyBrickCallBack);
            // Debug.Log($"Brick retrieved from pool. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnBrickToPool(GameObject brickGameObject)
        {
            Brick brick = brickGameObject.GetComponent<Brick>();
            brick.onDestroyed.RemoveAllListeners();
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

        #endregion
    }
}