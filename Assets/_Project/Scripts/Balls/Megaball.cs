using System.Collections;
using System.Collections.Generic;
using DaftAppleGames.RetroRacketRevolution.Balls;
using DaftAppleGames.RetroRacketRevolution.Bricks;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class Megaball : MonoBehaviour
    {

        private Ball _ball;

        private void Awake()
        {
            _ball = GetComponentInParent<Ball>();
        }

        private void Update()
        {
            Collider2D hitCollider = Physics2D.OverlapCircle(gameObject.transform.position, 0.165f, 1 << LayerMask.NameToLayer("Bricks"));
            if (hitCollider != null)
            {
                _ball.CollideWithBrick(hitCollider.gameObject.GetComponent<Brick>());
            }
        }
    }
}
