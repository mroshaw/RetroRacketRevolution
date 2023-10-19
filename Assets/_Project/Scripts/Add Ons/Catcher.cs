using DaftApplesGames.RetroRacketRevolution.Balls;
using DaftApplesGames.RetroRacketRevolution.Players;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class Catcher : AddOn
    {
        private Collider2D _collider;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Ball enters catcher trigger collider
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Ball"))
            {
                Ball ball = other.GetComponent<Ball>();
                ball.Attach(AttachedPlayer, _collider.bounds.ClosestPoint(other.transform.position));
            }
        }
    }
}
