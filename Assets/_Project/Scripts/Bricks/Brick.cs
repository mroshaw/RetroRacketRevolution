using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace DaftApplesGames.RetroRacketRevolution.Bricks
{
    public enum BrickType { Normal, Invincible, DoubleStrong, TripleStrong, DisruptorOut, DisruptorIn, DisruptorBoth }

    public class Brick : MonoBehaviour
    {
        [BoxGroup("Settings")] public Color brickColor;
        [BoxGroup("Settings")] public BrickType brickType;
        [BoxGroup("Settings")] public BonusType brickBonus;
        [BoxGroup("Events")] public UnityEvent<Brick> BrickDestroyedEvent;

        [BoxGroup("Sprites")] public SpriteRenderer spriteRenderer;
        [BoxGroup("Sprites")] public Sprite normalBrickSprite;
        [BoxGroup("Sprites")] public Sprite health2Sprite;
        [BoxGroup("Sprites")] public Sprite health1Sprite;
        [BoxGroup("Brick Type")] public GameObject normalBrickGameObject;
        [BoxGroup("Brick Type")] public GameObject invincibleBrickGameObject;
        [BoxGroup("Brick Type")] public SortingGroup invincibleBrickSortingGroup;

        [BoxGroup("Debug")] public int row;
        [BoxGroup("Debug")] public int col;

        // Public properties
        public BrickManager BrickManager { set; get; }

        // Private properties
        [BoxGroup("Debug")] [SerializeField] private int _health = 1;
        private int _scoreValue = 10;
        // Components
        private AudioSource _audioSource;

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
        /// Initialise the brick instance
        /// </summary>
        private void InitBrick()
        {
            // Set health
            switch (brickType)
            {
                case BrickType.Normal:
                    _health = 1;
                    spriteRenderer.sprite = normalBrickSprite;
                    invincibleBrickGameObject.SetActive(false);
                    normalBrickGameObject.SetActive(true);
                    break;
                case BrickType.DoubleStrong:
                    invincibleBrickGameObject.SetActive(false);
                    normalBrickGameObject.SetActive(true);
                    spriteRenderer.sprite = normalBrickSprite;
                    _health = 2;
                    break;
                case BrickType.TripleStrong:
                    invincibleBrickGameObject.SetActive(false);
                    normalBrickGameObject.SetActive(true);
                    spriteRenderer.sprite = normalBrickSprite;
                    _health = 3;
                    break;
                case BrickType.Invincible:
                    invincibleBrickGameObject.SetActive(true);
                    invincibleBrickGameObject.GetComponent<BrickGlint>().Reset();
                    normalBrickGameObject.SetActive(false);
                    _health = -1;
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

            // Set colour
            SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
            sprite.color = brickColor;
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
            _health--;

            // Change sprite for multi-hit bricks
            if (_health == 2 && health2Sprite)
            {
                spriteRenderer.sprite = health2Sprite;
            }

            if (_health == 1 && health1Sprite)
            {
                spriteRenderer.sprite = health1Sprite;
            }

            if (_health == 0)
            {
                // Add score to player who destroyed
                hitByPlayer.AddScore(_scoreValue);
                SpawnBonus();
                BrickDestroyedEvent.Invoke(this);
            }
        }

        /// <summary>
        /// Sets the sorting group for the invincible brick
        /// </summary>
        public void SetSortingGroup(bool mainGroup)
        {
            if (mainGroup)
            {
                invincibleBrickSortingGroup.sortingLayerName = "Brick1";
            }
            else
            {
                invincibleBrickSortingGroup.sortingLayerName = "Brick2";
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
            BonusManager.Instance.SpawnBonus(brickBonus, transform.position);
        }
    }
}
