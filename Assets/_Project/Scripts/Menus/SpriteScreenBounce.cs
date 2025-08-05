using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class SpriteScreenBounce : MonoBehaviour
    {
        private Rigidbody2D _rb;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            StartBouncing();
        }

        /// <summary>
        /// Check that sprite hasn't got stuck
        /// </summary>
        private void Update()
        {
            if (_rb.linearVelocity.x == 0)
            {
                _rb.linearVelocity = new Vector2(Random.Range(1, 3), _rb.linearVelocity.y);
            }

            if (_rb.linearVelocity.y == 0)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Random.Range(1, 3));
            }
        }

        /// <summary>
        /// Starts bouncing the sprite
        /// </summary>
        private void StartBouncing()
        {
            float rand = Random.Range(0, 2);
            _rb.AddForce(rand < 1 ? new Vector2(200, -150) : new Vector2(-200, -150));
        }
    }
}