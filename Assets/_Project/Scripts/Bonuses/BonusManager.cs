using System;
using System.Collections;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.AddOns;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public class BonusManager : MonoBehaviour
    {
        [BoxGroup("Prefabs")] [SerializeField] private Transform bonusContainer;
        [BoxGroup("Settings")] [SerializeField] [InlineEditor] private BonusData bonusData;
        [BoxGroup("Spawning")] [SerializeField] private Transform randomSpawnTransform;
        [BoxGroup("Spawning")] [SerializeField] private float bonusSpawnForce;
        [BoxGroup("Spawning")] [SerializeField] private Vector3 spawnAdjust;

        [FoldoutGroup("Events")] public UnityEvent<Bonus, GameObject> onBonusApplied;

        private List<Bonus> _bonuses;
        private AudioSource _audioSource;

        private Vector3 _spawnAdjust;

        /// <summary>
        /// Set up the Bonus Manager
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _bonuses = new();
        }

        /// <summary>
        /// Spawns a bonus at the given location
        /// </summary>
        public void SpawnBonus(BonusType bonusType, Vector3 spawnPosition)
        {
            BonusData.BonusDef bonusDef = bonusData.GetBonusByType(bonusType);
            if (bonusDef == null)
            {
                Debug.LogError($"Bonus Def not found for BonusType: {bonusType}");
                return;
            }

            GameObject newBonus = Instantiate(bonusData.GetBonusByType(bonusType).spawnPrefab, bonusContainer);
            Bonus bonus = newBonus.GetComponent<Bonus>();
            newBonus.transform.position = spawnPosition + spawnAdjust;
            bonus.MainBonusManager = this;
            bonus.Spawn();
            bonus.onDestroyed.AddListener(RemoveBonus);
            _bonuses.Add(bonus);
        }

        private void RemoveBonus(Bonus bonus)
        {
            _bonuses.Remove(bonus);
        }

        public void DestroyAllBonuses()
        {
            foreach (Bonus bonus in _bonuses.ToArray())
            {
                bonus.DestroyBonus();
            }
        }

        /// <summary>
        /// Wrapper for async method to remove the add-on
        /// </summary>
        private void RemoveBonusAddOnAfterDelay(HardPoint hardPoint, float delay)
        {
            StartCoroutine(RemoveBonusAddOnAfterDelayAsync(hardPoint, delay));
        }

        /// <summary>
        /// Removes add-on from hardpoint after a given delay
        /// </summary>
        private IEnumerator RemoveBonusAddOnAfterDelayAsync(HardPoint hardPoint, float delay)
        {
            yield return new WaitForSeconds(delay);
            hardPoint.Retract();
        }

        internal void SpawnRandomBonus()
        {
            BonusType randomBonusType = GetRandomBonus(BonusType.Random);
            SpawnBonus(randomBonusType, randomSpawnTransform.position + spawnAdjust);
        }

        /// <summary>
        /// Gets a random bonus
        /// </summary>
        private BonusType GetRandomBonus(BonusType excludeType)
        {
            Array values = Enum.GetValues(typeof(BonusType));
            System.Random random = new System.Random();
            int randomIndex = random.Next(1, values.Length - 1);

            if ((BonusType)randomIndex == excludeType)
            {
                randomIndex++;
            }

            BonusType randomBonus = (BonusType)values.GetValue(randomIndex);
            return randomBonus;
        }

        /// <summary>
        /// Wrapper around async run action after delay
        /// </summary>
        private void RunCallBackAfterDelay(float delay, Action callBack)
        {
            StartCoroutine(RunCallBackAfterDelayAsync(delay, callBack));
        }

        /// <summary>
        /// Runs the callback action after a delay
        /// </summary>
        /// <returns></returns>
        private IEnumerator RunCallBackAfterDelayAsync(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack.Invoke();
        }

        /// <summary>
        /// Bonus type and prefab pairs
        /// </summary>
        [Serializable] internal class BonusCollectable
        {
            public BonusType bonusType;
            public GameObject prefab;
        }
    }
}