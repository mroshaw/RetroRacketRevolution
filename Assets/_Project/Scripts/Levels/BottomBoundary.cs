using DaftApplesGames.RetroRacketRevolution.Balls;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public class BottomBoundary : MonoBehaviour
    {
        /// <summary>
        /// Handle things dropping out of the screen boundary
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollision2D(Collision2D collision)
        {
            GameObject collideGameObject = collision.gameObject;

            Ball ball = collideGameObject.GetComponent<Ball>();
            if(ball != null) { }
        }
    }
}
