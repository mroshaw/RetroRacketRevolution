using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    public enum BrickType { Normal, Invincible, DoubleStrong, TripleStrong, DisruptorOut, DisruptorIn, DisruptorBoth }

    public class Brick : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private Color brickColor;
        [BoxGroup("Settings")] [SerializeField] private BrickType brickType;
        [BoxGroup("Settings")] [SerializeField] private BonusType brickBonus;
        [BoxGroup("Events")] [SerializeField] public UnityEvent<Brick, bool> BrickDestroyedEvent;

        [BoxGroup("Debug")] [SerializeField] private int health = 1;
        [BoxGroup("Debug")] [SerializeField] private int row;
        [BoxGroup("Debug")] [SerializeField] private int col;

        // Public properties
        public BrickType BrickType => brickType;
        public Color BrickColor => brickColor;
        public BrickManager BrickManager { set; get; }
        public float Health => health;

        private Material _material;
        private int _scoreValue = 10;
        private AudioSource _audioSource;

        private static readonly int BrickMatColor = Shader.PropertyToID("_Color");

        private void OnEnable()
        {
            InitBrick();
        }

        /// <summary>
        /// Init components
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _material =  GetComponentInChildren<Renderer>().material;
        }

        /// <summary>
        /// Init the brick
        /// </summary>
        private void Start()
        {
            InitBrick();
        }

        /// <summary>
        /// Allow pooling to re-configure the brick
        /// </summary>
        public void OnSpawn()
        {
            InitBrick();
        }

        /// <summary>
        /// Reconfigure the brick
        /// </summary>
        public void ReConfigureBrick(BrickType newBrickType, Color newBrickColor, BonusType newBrickBonus)
        {
            brickType = newBrickType;
            brickColor = newBrickColor;
            brickBonus = newBrickBonus;
        }

        /// <summary>
        /// Initialise the brick instance
        /// </summary>
        private void InitBrick()
        {
            // Set health
            switch (brickType)
            {
                case BrickType.Normal:
                    health = 1;
                    break;
                case BrickType.DoubleStrong:
                    health = 2;
                    break;
                case BrickType.TripleStrong:
                    health = 3;
                    break;
                case BrickType.Invincible:
                    health = -1;
                    break;
            }

            // Set score value
            switch (brickType)
            {
                case BrickType.Normal:
                    _scoreValue = 100;
                    break;
                case BrickType.DoubleStrong:
                    _scoreValue = 300;
                    break;
                case BrickType.TripleStrong:
                    _scoreValue = 500;
                    break;
            }

            // Set color
            _material.SetColor(BrickMatColor, brickColor);

            // Init cracks
            if (IsDarkColor(brickColor))
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Brick has been hit
        /// </summary>
        public void BrickHit(Player hitByPlayer)
        {
            if (brickType == BrickType.Invincible)
            {
                return;
            }
            health--;

            // Change sprite for multi-hit bricks
            if (health == 2)
            {

            }

            if (health == 1)
            {

            }

            if (health == 0)
            {
                // Add score to player who destroyed
                hitByPlayer.AddScore(_scoreValue);
                SpawnBonus();
                BrickDestroyedEvent.Invoke(this, true);
            }
        }

        /// <summary>
        /// Sets the sorting group for the invincible brick
        /// </summary>
        public void SetSortingGroup(bool mainGroup)
        {
            if (mainGroup)
            {

            }
            else
            {

            }
        }

        /// <summary>
        /// Spawn any bonuses from a destroyed brick
        /// </summary>
        private void SpawnBonus()
        {
            if(brickBonus == BonusType.None)
            {
                return;
            }
            BrickManager.BrickSpawnBonus(brickBonus, transform.position);
            // BonusManager.Instance.SpawnBonus(brickBonus, transform.position);
        }

        /// <summary>
        /// Determine if color is dark
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private bool IsDarkColor(Color color)
        {
            return (color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722 < 255 / 2);
        }
    }
}