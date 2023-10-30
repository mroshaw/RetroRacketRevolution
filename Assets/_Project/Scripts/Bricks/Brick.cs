using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
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
        [BoxGroup("Events")] public UnityEvent<Brick, bool> BrickDestroyedEvent;

        [BoxGroup("Sprites")] public SpriteRenderer spriteRenderer;
        [BoxGroup("Sprites")] public Sprite normalBrickSprite;
        [BoxGroup("Sprites")] public Sprite doubleStrongSprite;
        [BoxGroup("Sprites")] public Sprite tripleStrongSprite;
        [BoxGroup("Sprites")] public Sprite invincibleSprite;

        [BoxGroup("Damage")] public SpriteRenderer health2SpriteRenderer;
        [BoxGroup("Damage")] public SpriteRenderer health1SpriteRenderer;

        [BoxGroup("Debug")] public int row;
        [BoxGroup("Debug")] public int col;

        // Public properties
        public BrickManager BrickManager { set; get; }
        public float Health => _health;

        // Private properties
        [BoxGroup("Debug")] [SerializeField] private int _health = 1;
        private int _scoreValue = 10;
        // Components
        private AudioSource _audioSource;
        private BrickGlint _brickGlint;

        private SortingGroup _spriteSortingGroup;
        private SortingGroup _glintSortingGroup;
        private SpriteMask _spriteMask;

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
            _spriteSortingGroup = spriteRenderer.GetComponent<SortingGroup>();
            _brickGlint = GetComponentInChildren<BrickGlint>(true);
            _glintSortingGroup = _brickGlint.GetComponentInChildren<SpriteRenderer>(true).GetComponent<SortingGroup>();
            _spriteMask = _brickGlint.GetComponentInChildren<SpriteMask>(true);
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
                    _brickGlint.gameObject.SetActive(false);
                    break;
                case BrickType.DoubleStrong:
                    spriteRenderer.sprite = doubleStrongSprite;
                    _brickGlint.gameObject.SetActive(false);
                    _health = 2;
                    break;
                case BrickType.TripleStrong:
                    spriteRenderer.sprite = tripleStrongSprite;
                    _brickGlint.gameObject.SetActive(false);
                    _health = 3;
                    break;
                case BrickType.Invincible:
                    spriteRenderer.sprite = invincibleSprite;
                    _brickGlint.gameObject.SetActive(true);
                    _brickGlint.Reset();
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

            // Init cracks
            health2SpriteRenderer.gameObject.SetActive(false);
            health1SpriteRenderer.gameObject.SetActive(false);
            if (IsDarkColor(brickColor))
            {
                health2SpriteRenderer.color = Color.white;
                health1SpriteRenderer.color = Color.white;
            }
            else
            {
                health2SpriteRenderer.color = Color.black;
                health1SpriteRenderer.color = Color.black;
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
            _health--;

            // Change sprite for multi-hit bricks
            if (_health == 2)
            {
                health2SpriteRenderer.gameObject.SetActive(true);
            }

            if (_health == 1)
            {
                health1SpriteRenderer.gameObject.SetActive(true);
            }

            if (_health == 0)
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
                _spriteSortingGroup.sortingLayerName = "Brick1";
                _glintSortingGroup.sortingLayerName = "Brick1";
                _spriteMask.frontSortingLayerID = SortingLayer.NameToID("Brick1");
            }
            else
            {
                _spriteSortingGroup.sortingLayerName = "Brick2";
                _glintSortingGroup.sortingLayerName = "Brick2";
                _spriteMask.frontSortingLayerID = SortingLayer.NameToID("Brick2");
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
