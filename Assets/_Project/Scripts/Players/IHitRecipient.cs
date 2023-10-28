using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Players
{
    public interface IHitRecipient
    {
        public void HitHandler(GameObject sourceGameObject, GameObject targetGameObject);
    }
}
