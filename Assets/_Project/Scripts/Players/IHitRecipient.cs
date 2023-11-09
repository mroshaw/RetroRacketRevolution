using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Players
{
    public interface IHitRecipient
    {
        public void HitHandler(GameObject sourceGameObject, GameObject targetGameObject);
    }
}
