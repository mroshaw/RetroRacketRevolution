using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public class LifeForce : MonoBehaviour
    {
        [BoxGroup("Settings")] public int defaultLives = 3;
        [FoldoutGroup("Events")] public UnityEvent AllLivesLostEvent;
        [FoldoutGroup("Events")] public UnityEvent LifeLostEvent;
        [FoldoutGroup("Events")] public UnityEvent<int> LivesUpdatedEvent;
        [BoxGroup("Cheats")] public bool UnlimitedLives { get; set; } = false;

        // Players lives
        public int NumLives
        {
            get => _numLives;
            set
            {
                _numLives = value;
                LivesUpdatedEvent.Invoke(value);
            }
        }

        private int _numLives = 0;


        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            NumLives = defaultLives;
        }

        /// <summary>
        /// Adds an extra life
        /// </summary>
        public void AddLife()
        {
            NumLives++;
        }

        /// <summary>
        /// Player loses a life
        /// </summary>
        public void LoseLife()
        {
            
            LifeLostEvent.Invoke();
            if (!UnlimitedLives)
            {
                NumLives--;
            }

            if (IsGameOver())
            {
                AllLivesLostEvent.Invoke();
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool IsGameOver()
        {
            return NumLives < 0;
        }
    }
}
