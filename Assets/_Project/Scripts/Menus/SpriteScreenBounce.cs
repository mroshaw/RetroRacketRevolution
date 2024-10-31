using UnityEngine;

namespace DaftAppleGames.Menus
{
    public class SpriteScreenBounce : MonoBehaviour
    {
        // Private fields
        private Rigidbody2D _rb;

        #region UnityMethods
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
        #endregion

        #region PublicMethods
        /// <summary>
        /// Starts bouncing the sprite
        /// </summary>
        public void StartBouncing()
        {
            float rand = Random.Range(0, 2);
            if (rand < 1)
            {
                _rb.AddForce(new Vector2(200, -150));
            }
            else
            {
                _rb.AddForce(new Vector2(-200, -150));

            }
        }
	    #endregion
    }
}
