using System.Linq;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public class Projectile : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private LayerMask colliderLayerMask;
        [BoxGroup("Settings")] [SerializeField] private string[] colliderTags;

        private Rigidbody _rb;

        public AddOn WeaponAddOn { get; set; }

        [FoldoutGroup("Events")] public UnityEvent<GameObject> projectileCollideEvent;

        /// <summary>
        /// Initialise the bolt
        /// </summary>
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

        }

        internal void Fire(float velocity)
        {
            _rb.AddForce(transform.up * velocity);
        }

        /// <summary>
        /// Handle laser hitting a brick, the ball or the screen
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"Projectile hit: {collision.gameObject.name}");

            if (colliderLayerMask != (colliderLayerMask | (1 << collision.gameObject.layer)))
            {
                Debug.Log($"Collider not in LayerMask");
                return;
            }

            if (!colliderTags.Any(collision.gameObject.tag.Contains))
            {
                Debug.Log($"Collider not in Tags");
                return;
            }

            // If collision with player, just ignore
            if (collision.gameObject.CompareTag("Player"))
            {
                return;
            }

            // Collided with brick
            if (collision.gameObject.CompareTag("Brick"))
            {
                Brick brick = collision.gameObject.GetComponent<Brick>();
                brick.BrickHit(WeaponAddOn.AttachedHardPoint.HardPointPlayer);
            }

            // Collided with enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.Hit(this.gameObject);
            }
            projectileCollideEvent.Invoke(this.gameObject);
        }
    }
}