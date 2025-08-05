using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftAppleGames.RetroRacketRevolution.Game
{
    public class DifficultyDescriptions : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("UI Settings")] [SerializeField] private TextMeshProUGUI easyText;
        [BoxGroup("UI Settings")] [SerializeField] private TextMeshProUGUI normalText;
        [BoxGroup("UI Settings")] [SerializeField] private TextMeshProUGUI hardText;
        [BoxGroup("UI Settings")] [SerializeField] private TextMeshProUGUI insaneText;
        [BoxGroup("UI Settings")] [SerializeField] private DifficultyData easyDifficultyData;
        [BoxGroup("UI Settings")] [SerializeField] private DifficultyData normalDifficultyData;
        [BoxGroup("UI Settings")] [SerializeField] private DifficultyData hardDifficultyData;
        [BoxGroup("UI Settings")] [SerializeField] private DifficultyData insaneDifficultyData;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            PopulateDescriptions();
        }

        /// <summary>
        /// Populate the difficulty descriptions
        /// </summary>
        private void PopulateDescriptions()
        {
            easyText.text = GetDescriptionText(easyDifficultyData);
            normalText.text = GetDescriptionText(normalDifficultyData);
            hardText.text = GetDescriptionText(hardDifficultyData);
            insaneText.text = GetDescriptionText(insaneDifficultyData);
        }

        /// <summary>
        /// Get description text for given difficulty
        /// </summary>
        /// <param name="difficultyData"></param>
        /// <returns></returns>
        private string GetDescriptionText(DifficultyData difficultyData)
        {
            string livesText = $"Lives: {difficultyData.startingLives}";
            string ballSpeedText = $"Ball speed: {difficultyData.defaultBallSpeed}";
            string ballSpeedUpDelayText = $"Ball speedup: {difficultyData.ballSpeedUpAfterDuration}s";
            string ballSpeedUpMultiplierText = $"Ball delta: {difficultyData.ballSpeedMultiplier}x";
            string batLengthText = $"Bat length: {difficultyData.defaultBatLength}";

            return
                $"{livesText}\n{ballSpeedText}\n{ballSpeedUpDelayText}\n{ballSpeedUpMultiplierText}\n{batLengthText}";
        }
    }
}