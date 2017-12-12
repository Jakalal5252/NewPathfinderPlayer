using System;

namespace NewPathfinderPlayer.Models
{
    public class CombatRound
    {
        [Flags]
        public enum CombatActions
        {
            Standard = 0,
            Move = 1,
            FullRound = 2,
            Swift = 3,
            Immediate = 4,
            Free = 5
        };
    }
}
