using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace DaftApplesGames.RetroRacketRevolution.Bricks
{
    public class BrickManager : MonoBehaviour
    {
        [BoxGroup("Audio")] public AudioClip destroyedClip;
        [BoxGroup("Prefabs")] public GameObject brickPrefab;
        [BoxGroup("Prefabs")] public GameObject disruptorPrefab;
        [BoxGroup("Prefabs")] public GameObject brickContainer;
        [BoxGroup("Prefabs")] public GameObject disruptorContainer;
        [BoxGroup("Prefabs")] public bool usePooling=false;
        [BoxGroup("Prefabs")] public int defaultPoolSize = 200;
        [BoxGroup("Debug")] [SerializeField] private List<Brick> bricks = new List<Brick>();
        [BoxGroup("Debug")] [SerializeField] private List<Disruptor> disruptors = new List<Disruptor>();
        [BoxGroup("Events")] public UnityEvent<Brick> BrickAddedEvent;
        [BoxGroup("Events")] public UnityEvent<Brick> BrickDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent LastBrickDestroyedEvent;
        [BoxGroup("Events")] public UnityEvent<BonusType, Vector2> BrickSpawnBonusEvent;

        // Private properties
        private AudioSource _audioSource;

        private int _destructableBricks;

        // Brick prefab pool
        [BoxGroup("Pool")] public ObjectPool<GameObject> brickPool;

        /// <summary>
        /// Set up the Brick Manager
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            if (usePooling)
            {
                brickPool = new ObjectPool<GameObject>(CreateBrick, OnTakeBrickFromPool, OnReturnBrickToPool, OnDestroyBrick, true, defaultPoolSize);
            }
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
                if (brick.brickType != BrickType.Invincible)
                {
                    DestroyBrickCallBack(brick, false);
                }
            }
        }

        /// <summary>
        /// Spawn a new brick with the given properties
        /// </summary>
        /// <param name="brickType"></param>
        /// <param name="brickColor"></param>
        /// <param name="brickBonus"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="isMainSortingGroup"></param>
        public Brick SpawnBrick(BrickType brickType, Color brickColor, BonusType brickBonus, int row, int col,
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
            brick.brickType = brickType;
            brick.brickColor = brickColor;
            brick.brickBonus = brickBonus;
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
        /// <param name="brickType"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
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
        public void DestroyAllDisruptors()
        {
            foreach (Disruptor currDisruptor in disruptors.ToArray())
            {
                DestroyDisruptor(currDisruptor);
            }
        }

        /// <summary>
        /// Destroys the given disruptor
        /// </summary>
        /// <param name="disruptor"></param>
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
        /// <param name="brick"></param>
        /// <param name="playSound"></param>
        public void DestroyBrickCallBack(Brick brick, bool playSound)
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
        /// <param name="brick"></param>
        /// <param name="playSound"></param>
        public void DestroyBrick(Brick brick, bool playSound)
        {
            if (brick.brickType != BrickType.Invincible && playSound)
            {
                _audioSource.PlayOneShot(destroyedClip);
            }
            bricks.Remove(brick);
            if (brick.brickType != BrickType.Invincible)
            {
                _destructableBricks--;
            }
            
            if (usePooling)
            {
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
        /// <returns></returns>
        private bool HasLastBrickBeenDestroyed()
        {
            foreach (Brick brick in bricks)
            {
                if (brick.brickType != BrickType.Invincible)
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
        /// <returns></returns>
        private GameObject CreateBrick()
        {
            GameObject newBrickGameObject = Instantiate(brickPrefab);
            newBrickGameObject.transform.parent = brickContainer.transform;
            Brick brick = newBrickGameObject.GetComponent<Brick>();
            brick.BrickManager = this;
            // Debug.Log($"Created brick in pool. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
            return newBrickGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="brickGameObject"></param>
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
        /// <param name="brickGameObject"></param>
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
        /// <param name="brickGameObject"></param>
        private void OnDestroyBrick(GameObject brickGameObject)
        {
            Brick brick = brickGameObject.GetComponent<Brick>();
            // Debug.Log($"Brick destroyed to pool. {brick.brickType}, {brick.brickColor}. Pool size is: {brickPool.CountAll} of which {brickPool.CountActive} are active.");
            Destroy(brickGameObject);
        }
        #endregion
    }
}
