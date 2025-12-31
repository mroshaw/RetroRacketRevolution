using System.Collections;
using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class Cheats : MonoBehaviour
    {
        [BoxGroup("Managers")] [SerializeField] private BrickManager brickManager;
        [BoxGroup("Managers")] [SerializeField] private BonusManager bonusManager;
        [BoxGroup("Managers")] [SerializeField] private PlayerManager playerManager;
        [BoxGroup("Player")] [SerializeField] private Player player1;
        [BoxGroup("Player")] [SerializeField] private Player player2;
        [BoxGroup("Cheat Settings")] [SerializeField] private float addOnDuration = 20.0f;
        [BoxGroup("Cheat Settings")] [SerializeField] private Transform cheatSpawnTransform;
        [BoxGroup("UI")] [SerializeField] private TextMeshProUGUI notificationText;
        [BoxGroup("UI")] [SerializeField] private float notificationFadeTime;
        [BoxGroup("UI")] [SerializeField] private float notificationVisibleTime;

        [FoldoutGroup("Events")] public UnityEvent onCheatsUsed;

        // Variables for notification fader
        private Color _visibleColor;
        private Color _hiddenColor;

        private AudioSource _audioSource;

        private bool _cheatsUsed;

        private void Awake()
        {
            Color textColor = notificationText.color;
            _visibleColor = new Color(textColor.r, textColor.g, textColor.b, 1);
            _hiddenColor = new Color(textColor.r, textColor.g, textColor.b, 0);
            notificationText.color = _visibleColor;
            _audioSource = GetComponent<AudioSource>();
            _cheatsUsed = false;
        }

        /// <summary>
        /// Handle CheatLives input message
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Toggle Infinite Lives")]
        internal void InfiniteLives()
        {
            // Toggle unlimited lives
            bool result = playerManager.ToggleUnlimitedLives();

            Notify($"Infinite lives is {result}");
        }

        /// <summary>
        /// 
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Give Laser (P1)")]
        internal void LaserP1()
        {
            HardPoint playerHardPoint = player1.GetFreeHardPoint(HardPointLocation.Outer);
            if (playerHardPoint != null)
            {
                playerHardPoint.Deploy();
                StartCoroutine(DisableAddOnAfterDelay(playerHardPoint, addOnDuration));
            }

            Notify($"Player 1 laser enabled for {addOnDuration} seconds");
        }

        /// <summary>
        /// 
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Give Laser (P2)")]
        internal void LaserP2()
        {
            HardPoint playerHardPoint = player2.GetFreeHardPoint(HardPointLocation.Outer);
            if (playerHardPoint != null)
            {
                playerHardPoint.Deploy();
                StartCoroutine(DisableAddOnAfterDelay(playerHardPoint, addOnDuration));
            }

            Notify($"Player 2 laser enabled for {addOnDuration} seconds");
        }

        /// <summary>
        /// 
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Give Catch (P1)")]
        internal void CatchP1()
        {
            HardPoint playerHardPoint = player1.GetFreeHardPoint(HardPointLocation.Center);
            if (playerHardPoint != null)
            {
                playerHardPoint.Deploy();
                StartCoroutine(DisableAddOnAfterDelay(playerHardPoint, addOnDuration));
            }

            Notify($"Player 1 catch enabled for {addOnDuration} seconds");
        }

        /// <summary>
        /// 
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Give Catch (P2)")]
        internal void CatchP2()
        {
            HardPoint playerHardPoint = player2.GetFreeHardPoint(HardPointLocation.Center);
            if (playerHardPoint != null)
            {
                playerHardPoint.Deploy();
                StartCoroutine(DisableAddOnAfterDelay(playerHardPoint, addOnDuration));
            }

            Notify($"Player 2 catch enabled for {addOnDuration} seconds");
        }

        /// <summary>
        /// Handle Spawn cheat message (life)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Extra Life")]
        internal void SpawnLife()
        {
            bonusManager.SpawnBonus(BonusType.ExtraLife, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.ExtraLife}");
        }

        /// <summary>
        /// Handle Spawn cheat message (laser)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Laser")]
        internal void SpawnLaser()
        {
            bonusManager.SpawnBonus(BonusType.Laser, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.Laser}");
        }

        /// <summary>
        /// Handle Spawn cheat message (catch)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Catch")]
        internal void SpawnCatch()
        {
            bonusManager.SpawnBonus(BonusType.Catcher, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.Catcher}");
        }

        /// <summary>
        /// Handle Spawn cheat message (slow ball)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Slow Ball")]
        internal void SpawnSlowBall()
        {
            bonusManager.SpawnBonus(BonusType.SlowBall, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.SlowBall}");
        }

        /// <summary>
        /// Handle Spawn cheat message (mega ball)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Mega Ball")]
        internal void SpawnMegaBall()
        {
            bonusManager.SpawnBonus(BonusType.MegaBall, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.MegaBall}");
        }

        /// <summary>
        /// Handle Spawn cheat message (multi ball)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Multi Ball")]
        internal void SpawnMultiBall()
        {
            bonusManager.SpawnBonus(BonusType.MultiBall, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.MultiBall}");
        }

        /// <summary>
        /// Handle Spawn cheat message (grow bat)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Grow Bat")]
        internal void SpawnGrowBat()
        {
            bonusManager.SpawnBonus(BonusType.GrowBat, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.GrowBat}");
        }

        /// <summary>
        /// Handle Spawn cheat message (shrink bat)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Shrink Bat")]
        internal void SpawnShrinkBat()
        {
            bonusManager.SpawnBonus(BonusType.ShrinkBat, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.ShrinkBat}");
        }

        /// <summary>
        /// Handle Spawn cheat message (small score)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Small Score")]
        internal void SpawnSmallScore()
        {
            bonusManager.SpawnBonus(BonusType.SmallScore, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.SmallScore}");
        }

        /// <summary>
        /// Handle Spawn cheat message (big score)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn BigScore")]
        internal void SpawnBigScore()
        {
            bonusManager.SpawnBonus(BonusType.BigScore, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.BigScore}");
        }

        /// <summary>
        /// Handle Spawn cheat message (win level)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Win Level")]
        internal void SpawnWinLevel()
        {
            bonusManager.SpawnBonus(BonusType.FinishLevel, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.FinishLevel}");
        }

        /// <summary>
        /// Handle Spawn cheat message (random)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("Spawn Random")]
        internal void SpawnRandom()
        {
            bonusManager.SpawnBonus(BonusType.Random, cheatSpawnTransform.position);
            Notify($"Spawned {BonusType.Random}");
        }

        /// <summary>
        /// Handle Skip Level cheat message (random)
        /// </summary>
        [FoldoutGroup("Cheats")]
        [Button("SkipLevel")]
        internal void SkipLevel()
        {
            brickManager.DestroyAllBricks();
            Notify($"Skipping level...");
        }

        /// <summary>
        /// Add a notification to the queue and show
        /// </summary>
        private void Notify(string message)
        {
            // Notify any listeners that cheats have been used
            if (!_cheatsUsed)
            {
                _cheatsUsed = true;
                onCheatsUsed.Invoke();
            }

            // Notify the player
            _audioSource.Play();
            StartCoroutine(NotifyFade(message));
        }

        /// <summary>
        /// Show the current notification queue, fade in and out
        /// </summary>
        private IEnumerator NotifyFade(string message)
        {
            notificationText.color = _hiddenColor;
            notificationText.text = message;

            // Fade text in
            float time = 0;
            while (time < notificationFadeTime)
            {
                notificationText.color = Color.Lerp(_hiddenColor, _visibleColor, time / notificationFadeTime);
                time += Time.deltaTime;
                yield return null;
            }

            notificationText.color = _visibleColor;

            // Wait
            yield return new WaitForSecondsRealtime(notificationVisibleTime);

            // Fade out
            time = 0;
            while (time < notificationFadeTime)
            {
                notificationText.color = Color.Lerp(_visibleColor, _hiddenColor, time / notificationFadeTime);
                time += Time.deltaTime;
                yield return null;
            }

            notificationText.color = _hiddenColor;
            notificationText.text = "";
        }

        /// <summary>
        /// Disable AddOn cheats after time
        /// </summary>
        private IEnumerator DisableAddOnAfterDelay(HardPoint hardPoint, float delay)
        {
            yield return new WaitForSeconds(delay);
            hardPoint.Retract();
        }
    }
}