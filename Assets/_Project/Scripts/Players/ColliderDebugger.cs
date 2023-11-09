using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public class ColliderDebugger : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log($"ColliderDebugger: OnCollisionEnter2D called with: {collision.gameObject}");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"ColliderDebugger: OnTriggerEnter2D called with: {other.gameObject}");
        }

    }
}
