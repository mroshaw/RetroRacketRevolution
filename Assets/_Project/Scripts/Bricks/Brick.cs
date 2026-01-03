using DaftAppleGames.RetroRacketRevolution.Bonuses;
using DaftAppleGames.RetroRacketRevolution.Effects;
using DaftAppleGames.RetroRacketRevolution.Players;
using DaftAppleGames.RetroRacketRevolution.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.Bricks
{
    public enum BrickType
    {
        Normal,
        Invincible,
        DoubleStrong,
        TripleStrong,
        DisruptorOut,
        DisruptorIn,
        DisruptorBoth
    }

    public class Brick : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private Transform brickModel;
        [BoxGroup("Settings")] [SerializeField] private Transform normalModel;
        [BoxGroup("Settings")] [SerializeField] private Transform cracked1Model;
        [BoxGroup("Settings")] [SerializeField] private Transform cracked2Model;
        [BoxGroup("Settings")] [SerializeField] private Transform invincibleModel;

        [BoxGroup("Settings")] [SerializeField] private Color brickColor;
        [BoxGroup("Settings")] [SerializeField] private BrickType brickType;
        [BoxGroup("Settings")] [SerializeField] private BonusType brickBonus;
        [BoxGroup("Settings")] [SerializeField] private string normalLayer = "Bricks";
        [BoxGroup("Settings")] [SerializeField] private string invincibleLayer = "InvincibleBricks";
        [BoxGroup("Events")] [SerializeField] public UnityEvent<Brick> onDestroyed;

        [BoxGroup("Debug")] [SerializeField] private int health = 1;
        [BoxGroup("Debug")] [SerializeField] private int row;
        [BoxGroup("Debug")] [SerializeField] private int col;

        private MaterialTools _matTools;
        private Explosion _explosion;

        // Public properties
        public BrickType BrickType => brickType;
        public Color BrickColor => brickColor;
        public BrickManager BrickManager { set; get; }
        public int Health => health;
        private int _startingHealth;
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
            _material = GetComponentInChildren<Renderer>().material;
            _explosion = GetComponentInChildren<Explosion>();
            _matTools = GetComponent<MaterialTools>();
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
            brickModel.gameObject.SetActive(true);

            // Set health
            switch (brickType)
            {
                case BrickType.Normal:
                    _startingHealth = 1;
                    health = 1;
                    gameObject.layer = LayerMask.NameToLayer(normalLayer);
                    SetModel(normalModel);
                    break;
                case BrickType.DoubleStrong:
                    _startingHealth = 2;
                    health = 2;
                    gameObject.layer = LayerMask.NameToLayer(normalLayer);
                    SetModel(normalModel);
                    break;
                case BrickType.TripleStrong:
                    _startingHealth = 3;
                    health = 3;
                    gameObject.layer = LayerMask.NameToLayer(normalLayer);
                    SetModel(normalModel);
                    break;
                case BrickType.Invincible:
                    _startingHealth = -1;
                    health = -1;
                    gameObject.layer = LayerMask.NameToLayer(invincibleLayer);
                    SetModel(invincibleModel);
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
            // _material.SetColor(BrickMatColor, brickColor);
            _matTools.SetColor(brickColor);
        }

        /// <summary>
        /// Sets the given model as the active model
        /// </summary>
        private void SetModel(Transform model)
        {
            normalModel.gameObject.SetActive(false);
            cracked1Model.gameObject.SetActive(false);
            cracked2Model.gameObject.SetActive(false);
            invincibleModel.gameObject.SetActive(false);

            model.gameObject.SetActive(true);
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

            // Set the appropriate model for multi-hit bricks
            switch (health)
            {
                case 2:
                    SetModel(cracked1Model);
                    break;
                case 1:
                    SetModel(cracked2Model);
                    break;
            }

            if (health == 0)
            {
                // Add score to player who destroyed
                Destroy();
                hitByPlayer.AddScore(_scoreValue);
                SpawnBonus();
            }
        }

        /// <summary>
        /// Destroy the brick
        /// </summary>
        internal void Destroy(bool playSound = true)
        {
            brickModel.gameObject.SetActive(false);
            _explosion.Explode(true);
            onDestroyed.Invoke(this);
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
            if (brickBonus == BonusType.None)
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