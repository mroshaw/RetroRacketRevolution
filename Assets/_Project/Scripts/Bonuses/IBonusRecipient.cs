using DaftAppleGames.RetroRacketRevolution.Bonuses;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Bonuses
{
    public interface IBonusRecipient
    {
        public void ApplyBonus(Bonus bonus);
    }
}