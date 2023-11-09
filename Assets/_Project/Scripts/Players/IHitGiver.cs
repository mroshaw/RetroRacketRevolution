using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public interface IHitGiver
    {
        public void Hit(GameObject sourceGameObject, GameObject targetGameObject);
    }
}
