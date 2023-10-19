using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DaftApplesGames.RetroRacketRevolution.Balls;
using DaftApplesGames.RetroRacketRevolution.Levels;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Bonuses
{
    public enum BonusType { None, MultiBall, Laser, SlowBall, MegaBall, ExtraLife, FinishLevel, Random, SmallScore, BigScore, ShrinkBat, GrowBat, Catcher }

    public class BonusManager : MonoBehaviour
    {
        public static BonusManager Instance { get; private set; }

        [BoxGroup("Managers")] public BallManager ballManager;
        [BoxGroup("Managers")] public AddOnManager addOnManager;
        [BoxGroup("Managers")] public PlayerManager playerManager;
        [BoxGroup("Managers")] public LevelLoader levelLoader;
        [BoxGroup("Prefabs")] public Transform bonusContainer;
        [BoxGroup("Prefabs")] public GameObject bonusPrefab;
        [BoxGroup("Settings")] public BonusCollectable[] bonusItemArray;

        private Dictionary<BonusType, BonusCollectable> bonusItemDict;

        private AudioSource _audioSource;

        [SerializeField]
        private Dictionary<BonusType, BonusCollectable> _bonusDict = new Dictionary<BonusType, BonusCollectable>();

        [BoxGroup("Settings")] public GameObject container;

        /// <summary>
        /// Set up the Bonus Manager
        /// </summary>
        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;

                // Convert the array to dictionary for easier/quicker
                // lookup
                bonusItemDict = bonusItemArray.ToDictionary(x => x.BonusType);
                _audioSource = GetComponent<AudioSource>();
            }
        }

        /// <summary>
        /// Spawns a bonus at the given location
        /// </summary>
        /// <param name="bonusType"></param>
        /// <param name="spawnPosition"></param>
        public void SpawnBonus(BonusType bonusType, Vector2 spawnPosition)
        {
            GameObject newBonus = Instantiate(bonusItemDict[bonusType].Prefab, bonusContainer);
            Bonus bonus = newBonus.GetComponent<Bonus>();
            newBonus.transform.position = spawnPosition;
            bonus.MainBonusManager = this;
            if (bonus.spawnAudioClip)
            {
                _audioSource.PlayOneShot(bonus.spawnAudioClip);
            }
        }

        /// <summary>
        /// Applies the bonus effect on the target
        /// </summary>
        /// <param name="bonus"></param>
        /// <param name="targetGameObject"></param>
        public void ApplyBonusEffect(Bonus bonus, GameObject targetGameObject)
        {
            // Play bonus audio
            _audioSource.PlayOneShot(bonus.collectAudioClip);

            Player player = targetGameObject.GetComponentInParent<Player>();

            // Process the bonus
            switch (bonus.bonusType)
            {
                case BonusType.MultiBall:
                    ballManager.SpawnTripleBall();
                    break;
                case BonusType.ExtraLife:
                    playerManager.AddLife();
                    break;
                case BonusType.SlowBall:
                    ballManager.SlowAllBalls();
                    break;
                case BonusType.SmallScore:
                case BonusType.BigScore:
                player.AddScore(bonus.scoreToAdd);
                    break;
                case BonusType.MegaBall:
                    ballManager.MakeMegaBalls();
                    break;
                case BonusType.Laser:
                case BonusType.Catcher:
                case BonusType.FinishLevel:
                    HardPoint playerHardPoint = player.GetFreeHardPoint(bonus.hardPointLocation);
                    if (playerHardPoint != null)
                    {
                        playerHardPoint.EnableAddOn();
                        if (bonus.duration > 0.0f)
                        {
                            RemoveBonusAddOnAfterDelay(playerHardPoint, bonus.duration);
                        }
                    }
                    break;
                case BonusType.ShrinkBat:
                    player.ShrinkBat();
                    break;
                case BonusType.GrowBat:
                    player.GrowBat();
                    break;
            }
        }

        /// <summary>
        /// Wrapper for async method to remove the add-on
        /// </summary>
        /// <param name="hardPoint"></param>
        /// <param name="delay"></param>
        private void RemoveBonusAddOnAfterDelay(HardPoint hardPoint, float delay)
        {
            StartCoroutine(RemoveBonusAddOnAfterDelayAsync(hardPoint, delay));
        }

        /// <summary>
        /// Removes add-on from hardpoint after a given delay
        /// </summary>
        /// <param name="hardPoint"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private IEnumerator RemoveBonusAddOnAfterDelayAsync(HardPoint hardPoint, float delay)
        {
            yield return new WaitForSeconds(delay);
            // hardPoint.DetachAddOn(true, true);
            hardPoint.DisableAddOn();
        }

        /// <summary>
        /// Gets a random bonus
        /// </summary>
        /// <returns></returns>
        private BonusType GetRandomBonus()
        {
            Array values = Enum.GetValues(typeof(BonusType));
            System.Random random = new System.Random();
            BonusType randomBonus = (BonusType)values.GetValue(random.Next(values.Length));
            return randomBonus;
        }

        /// <summary>
        /// Wrapper around async run action after delay
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="callBack"></param>
        private void RunCallBackAfterDelay(float delay, Action callBack)
        {
            StartCoroutine(RunCallBackAfterDelayAsync(delay, callBack));
        }

        /// <summary>
        /// Runs the callback action after a delay
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private IEnumerator RunCallBackAfterDelayAsync(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack.Invoke();
        }

        /// <summary>
        /// Bonus type and prefab pairs
        /// </summary>
        [Serializable]
        public class BonusCollectable
        {
            public BonusType BonusType;
            public GameObject Prefab;
        }

        #region PoolRegion
        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateBonus()
        {
            GameObject newBonusGameObject = Instantiate(bonusPrefab);
            newBonusGameObject.transform.parent = bonusContainer.transform;
            Bonus bonus = newBonusGameObject.GetComponent<Bonus>();
            bonus.MainBonusManager = this;
            return newBonusGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="bonus"></param>
        private void OnTakeBonusFromPool(GameObject bonus)
        {
            bonus.SetActive(true);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        /// <param name="bonus"></param>
        private void OnReturnBonusToPool(GameObject bonus)
        {
            bonus.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        /// <param name="bonus"></param>
        private void OnDestroyBonus(GameObject bonus)
        {
            Destroy(bonus);
        }
        #endregion
    }
}
