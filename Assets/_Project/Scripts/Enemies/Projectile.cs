using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Projectile : MonoBehaviour
    {

        [BoxGroup("Audio")] public AudioClip fallingAudioClip;
        [FoldoutGroup("Events")] public UnityEvent<Projectile> ProjectileDestroyedEvent;

        public EnemyManager EnemyManager { get; set; }

        private AudioSource _audioSource;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Set up components
        /// </summary>
        private void Start()
        {

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

        /// <summary>
        /// Call this when spawning / fetching from pool
        /// </summary>
        public void OnSpawn()
        {
            _audioSource.PlayOneShot(fallingAudioClip);
        }
    }
}
