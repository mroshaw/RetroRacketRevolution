using System.Collections;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using DaftAppleGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class LaserBolt : MonoBehaviour
    {
        [BoxGroup("Laser Bolt Settings")] public float speed = 2.0f;
        [BoxGroup("Laser Bolt Settings")] public SpriteRenderer spriteRenderer;
        [BoxGroup("Laser Bolt Settings")] public Sprite[] boltSprites;

        private int _numSprites;
        private int _randomSprite;
        private Rigidbody2D _rb;

        public LaserCannon LaserCannon { get; set; }

        [FoldoutGroup("Events")] public UnityEvent<GameObject> LaserBoltCollideEvent;

        /// <summary>
        /// Initialise the bolt
        /// </summary>
        private void Awake()
        {
            _numSprites = boltSprites.Length;
            System.Random rand = new System.Random();
            _randomSprite = rand.Next(0, boltSprites.Length - 1);
            spriteRenderer.sprite = boltSprites[_randomSprite];
            _rb = GetComponent<Rigidbody2D>();

        }

        /// <summary>
        /// Fire on start
        /// </summary>
        private void Start()
        {
            _rb.linearVelocity = Vector3.up * speed;
        }
/*
        /// <summary>
        /// Move the bolt upwards
        /// </summary>
        private void Update()
        {
            if (_isMoving)
            {
                transform.Translate(0, speed * Time.deltaTime, 0);
            }
        }
*/
        /// <summary>
        /// Handle laser hitting a brick, the ball or the screen
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // If collision with player, just ignore
            if (collision.gameObject.CompareTag("Player"))
            {
                return;
            }

            // Collided with brick
            if (collision.gameObject.CompareTag("Brick"))
            {
                Brick brick = collision.gameObject.GetComponent<Brick>();
                brick.BrickHit(LaserCannon.AttachedHardPoint.HardPointPlayer);
            }

            // Collided with enemy
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.Hit(this.gameObject);
            }
            LaserBoltCollideEvent.Invoke(this.gameObject);
        }
    }
}
