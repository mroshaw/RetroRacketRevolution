using DaftAppleGames.RetroRacketRevolution.AddOns;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class EnemyProjectile : Projectile
    {

        [BoxGroup("Audio")] [SerializeField] private AudioClip fallingAudioClip;
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
        /// Initialise the projectile
        /// </summary>
        public void InitProjectile(float scale)
        {
            // Set the scale
            gameObject.transform.localScale = new Vector3(scale, scale, scale);
            _audioSource.PlayOneShot(fallingAudioClip);
        }

        /// <summary>
        /// Check for collision with player
        /// </summary>
        private void OnCollisionEnter(Collision collision)
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
        private void OnTriggerEnter(Collider collision)
        {
            // If hit bottom boundary
            if (collision.gameObject.CompareTag("OutOfBounds"))
            {
                ProjectileDestroyedEvent.Invoke(this);
            }
        }
    }
}