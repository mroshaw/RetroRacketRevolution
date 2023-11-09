using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public class BonusManager : MonoBehaviour
    {
        [BoxGroup("Prefabs")] public Transform bonusContainer;
        [BoxGroup("Settings")] public BonusData bonusData;
        [BoxGroup("Settings")] public GameObject container;
        [BoxGroup("Random")] public Vector2 randomSpawnPosition;

        [FoldoutGroup("Events")] public UnityEvent<Bonus, GameObject> BonusAppliedEvent;

        private AudioSource _audioSource;

        /// <summary>
        /// Set up the Bonus Manager
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Spawns a bonus at the given location
        /// </summary>
        /// <param name="bonusType"></param>
        /// <param name="spawnPosition"></param>
        public void SpawnBonus(BonusType bonusType, Vector2 spawnPosition)
        {
            GameObject newBonus = Instantiate(bonusData.GetBonusByType(bonusType).SpawnPrefab, bonusContainer);
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

            // If Random, spawn a random bonus
            if (bonus.bonusType == BonusType.Random)
            {
                BonusType randomBonus = GetRandomBonus(BonusType.Random);
                // Debug.Log($"Spawning random bonus... {randomBonus.ToString()}");
                SpawnBonus(randomBonus, randomSpawnPosition);
                return;
            }

            BonusAppliedEvent.Invoke(bonus, targetGameObject);
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
    }
}
