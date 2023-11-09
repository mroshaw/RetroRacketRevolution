using DaftAppleGames.RetroRacketRevolution.Bonuses;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution
{
    public interface IBonusRecipient
    {
        public void BonusAppliedHandler(Bonus bonus, GameObject targetGameObject);
    }
}
