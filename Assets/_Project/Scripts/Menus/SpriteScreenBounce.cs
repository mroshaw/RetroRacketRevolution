using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

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
            if (_rb.velocity.x == 0)
            {
                _rb.velocity = new Vector2(Random.Range(1, 3), _rb.velocity.y);
            }

            if (_rb.velocity.y == 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, Random.Range(1, 3));
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
