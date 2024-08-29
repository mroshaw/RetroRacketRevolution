using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Projectile : MonoBehaviour
    {

        [BoxGroup("Audio")] public AudioClip fallingAudioClip;
        [BoxGroup("Visual")] public Sprite sprite;
        [FoldoutGroup("Events")] public UnityEvent<Projectile> ProjectileDestroyedEvent;

        public EnemyManager EnemyManager { get; set; }

        private AudioSource _audioSource;
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        /// <summary>
        /// Initialise the projectile
        /// </summary>
        /// <param name="projectileSprite"></param>
        /// <param name="projectilveFallingAudioClip"></param>
        /// <param name="scale"></param>
        public void InitProjectile(Sprite projectileSprite, AudioClip projectilveFallingAudioClip, float scale)
        {
            // Set the sp[rite, audio and scale
            _spriteRenderer.sprite = projectileSprite;
            fallingAudioClip = projectilveFallingAudioClip;
            gameObject.transform.localScale = new Vector2(scale, scale);

            // Set the collider size
            _collider.size = new Vector2(_spriteRenderer.size.x, _spriteRenderer.size.y);

            _audioSource.PlayOneShot(fallingAudioClip);
        }

        /// <summary>
        /// Check for collision with player
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player2"))
            {
                Player player = collision.gameObject.GetComponentInParent<Player>();
                player.Hit();
                ProjectileDestroyedEvent.Invoke(this);
            }
        }

        /// <summary>
        /// Handle collisions with the boundary triggers
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // If hit bottom boundary
            if (collision.gameObject.CompareTag("OutOfBounds"))
            {
                ProjectileDestroyedEvent.Invoke(this);
            }

        }
    }
}
