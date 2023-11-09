using DaftAppleGames.RetroRacketRevolution.Game;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class DifficultyWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("Difficulty Settings")] public DifficultyData easyDifficultyData;
        [BoxGroup("Difficulty Settings")] public DifficultyData normalDifficultyData;
        [BoxGroup("Difficulty Settings")] public DifficultyData hardDifficultyData;
        [BoxGroup("Difficulty Settings")] public DifficultyData insaneDifficultyData;
        [BoxGroup("Game Data")] public GameData gameData;
        
        [FoldoutGroup("Events")]
        public UnityEvent DifficultySelectedEvent;

        // Public properties
        
        // Private fields

        #region UnityMethods
        #endregion

	    #region PublicMethods

        /// <summary>
        /// Handle click of "Easy" button
        /// </summary>
        public void EasySelect()
        {
            gameData.difficulty = easyDifficultyData;
            DifficultySelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Normal" button
        /// </summary>
        public void NormalSelect()
        {
            gameData.difficulty = normalDifficultyData;
            DifficultySelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Hard" button
        /// </summary>
        public void HardSelect()
        {
            gameData.difficulty = hardDifficultyData;
            DifficultySelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Insane" button
        /// </summary>
        public void InsaneSelect()
        {
            gameData.difficulty = insaneDifficultyData;
            DifficultySelectedEvent.Invoke();
        }
        #endregion

	    #region PrivateMethods
	    
	    #endregion
    }
}
