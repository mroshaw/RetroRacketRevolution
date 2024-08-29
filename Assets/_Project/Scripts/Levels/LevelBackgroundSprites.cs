using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    [CreateAssetMenu(fileName = "BackgroundSprites", menuName = "Level Editor/BackgroundSprites", order = 1)]
    public class LevelBackgroundSprites : ScriptableObject
    {
        public List<Sprite> BackgroundSprites;

        /// <summary>
        /// De-dupe the list
        /// </summary>
        [Button("De-Dupe")]
        public void DeDupe()
        {
            List<Sprite> newList = new List<Sprite>();

            foreach (Sprite sprite in BackgroundSprites)
            {
                if(!FindItemByName(newList, sprite.name))
                {
                    newList.Add(sprite);
                }
            }
            BackgroundSprites = newList;
            // Debug.Log($"Total: {newList.Count}");
        }

        /// <summary>
        /// Find a Sprite by name in the given list
        /// </summary>
        /// <param name="listOfSprites"></param>
        /// <param name="nameToFind"></param>
        /// <returns></returns>
        private bool FindItemByName(List<Sprite> listOfSprites, string nameToFind)
        {
            foreach (Sprite sprite in listOfSprites)
            {
                if (sprite.name == nameToFind)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
