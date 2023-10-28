using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public interface IHitGiver
    {
        public void Hit(GameObject sourceGameObject, GameObject targetGameObject);
    }
}
