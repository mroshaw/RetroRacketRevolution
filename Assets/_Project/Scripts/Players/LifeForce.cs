using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class LifeForce : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private int defaultLives = 3;
        [FoldoutGroup("Events")] public UnityEvent onAllLivesLost;
        [FoldoutGroup("Events")] public UnityEvent onLifeLost;
        [FoldoutGroup("Events")] public UnityEvent<int> onLivesUpdated;
        [BoxGroup("Cheats")] public bool UnlimitedLives { get; set; } = false;

        // Players lives
        public int NumLives
        {
            get => _numLives;
            set
            {
                _numLives = value;
                onLivesUpdated.Invoke(value);
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
            onLifeLost.Invoke();
            if (!UnlimitedLives)
            {
                NumLives--;
            }

            if (IsGameOver())
            {
                onAllLivesLost.Invoke();
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