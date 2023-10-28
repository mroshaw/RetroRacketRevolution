using DaftApplesGames.RetroRacketRevolution.Bonuses;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution
{
    public interface IBonusRecipient
    {
        public void BonusAppliedHandler(Bonus bonus, GameObject targetGameObject);
    }
}
