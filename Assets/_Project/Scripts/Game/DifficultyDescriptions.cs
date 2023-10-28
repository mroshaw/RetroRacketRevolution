using DaftAppleGames.RetroRacketRevolution.Game;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftAppleGames.Game
{
    public class DifficultyDescriptions : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("UI Settings")] public TextMeshProUGUI easyText;
        [BoxGroup("UI Settings")] public TextMeshProUGUI normalText;
        [BoxGroup("UI Settings")] public TextMeshProUGUI hardText;
        [BoxGroup("UI Settings")] public TextMeshProUGUI insaneText;
        [BoxGroup("UI Settings")] public DifficultyData easyDifficultyData;
        [BoxGroup("UI Settings")] public DifficultyData normalDifficultyData;
        [BoxGroup("UI Settings")] public DifficultyData hardDifficultyData;
        [BoxGroup("UI Settings")] public DifficultyData insaneDifficultyData;

        [FoldoutGroup("Events")]
        public UnityEvent MyEvent;

        // Public properties
        
        // Private fields

        #region UnityMethods

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            PopulateDescriptions();
        }
        #endregion

	    #region PublicMethods
        /// <summary>
        /// Populate the difficulty descriptions
        /// </summary>
        public void PopulateDescriptions()
        {
            easyText.text = GetDescriptionText(easyDifficultyData);
            normalText.text = GetDescriptionText(normalDifficultyData);
            hardText.text = GetDescriptionText(hardDifficultyData);
            insaneText.text = GetDescriptionText(insaneDifficultyData);
        }
	    #endregion

	    #region PrivateMethods

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

            return $"{livesText}\n{ballSpeedText}\n{ballSpeedUpDelayText}\n{ballSpeedUpMultiplierText}\n{batLengthText}";

        }
	    #endregion
    }
}
