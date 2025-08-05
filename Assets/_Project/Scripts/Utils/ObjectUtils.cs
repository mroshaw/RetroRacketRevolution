using System.Collections;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Utils
{
    public static class ObjectUtils
    {
        public class MonoClass : MonoBehaviour { }

        private static MonoClass _monoClassInstance;

        /// <summary>
        /// Init the MonoBehaviour so we can use StartCoroutine
        /// </summary>
        private static void Init()
        {
            if (_monoClassInstance == null)
            {
                GameObject objectUtilsGameObject = new GameObject("ObjectUtils");
                _monoClassInstance = objectUtilsGameObject.AddComponent<MonoClass>();
            }
        }

        /// <summary>
        /// Sets the object active state after a given delay
        /// </summary>
        /// <param name="gameObjectToDisable"></param>
        /// <param name="activeState"></param>
        /// <param name="delay"></param>
        public static void SetActiveStateAfterDelay(GameObject gameObjectToDisable, bool activeState, float delay)
        {
            Init();
            _monoClassInstance.StartCoroutine(SetActiveStateAfterDelayAsync(gameObjectToDisable, activeState, delay));
        }

        /// <summary>
        /// Sets the object active state after a given delay
        /// </summary>
        /// <param name="objectToDisable"></param>
        /// <param name="activeState"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        private static IEnumerator SetActiveStateAfterDelayAsync(GameObject objectToDisable, bool activeState, float delay)
        {
            yield return new WaitForSeconds(delay);
            objectToDisable.SetActive(false);
        }
    }
}
