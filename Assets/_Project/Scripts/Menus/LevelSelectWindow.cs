using DaftAppleGames.RetroRacketRevolution.Game;
using DaftApplesGames.RetroRacketRevolution;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace DaftAppleGames.Menus
{
    public class LevelSelectWindow : WindowBase
    {
        // Public serializable properties
        [BoxGroup("Game Data")] public GameData gameData;
        
        [FoldoutGroup("Events")]
        public UnityEvent LevelSelectedEvent;

        // Public properties
        
        // Private fields

        #region UnityMethods
        #endregion

	    #region PublicMethods

        /// <summary>
        /// Handle click of "Easy" button
        /// </summary>
        public void OgSelect()
        {
            gameData.levelSelect = LevelSelect.Original;
            LevelSelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Normal" button
        /// </summary>
        public void CustomSelect()
        {
            gameData.levelSelect = LevelSelect.Custom;
            LevelSelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Hard" button
        /// </summary>
        public void OgPlusCustomSelect()
        {
            gameData.levelSelect = LevelSelect.OgPlusCustom;
            LevelSelectedEvent.Invoke();
        }

        /// <summary>
        /// Handle click of "Insane" button
        /// </summary>
        public void CustomPlusOgSelect()
        {
            gameData.levelSelect = LevelSelect.CustomPlusOg;
            LevelSelectedEvent.Invoke();
        }
        #endregion

	    #region PrivateMethods
	    
	    #endregion
    }
}
