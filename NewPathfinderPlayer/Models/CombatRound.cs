using System;

namespace NewPathfinderPlayer.Models
{
    public class CombatRound
    {
        public string Name = "";
        [Flags]
        public enum CombatActions
        {
            Standard = 0,
            Move = 1,
            //FullRound = 2,
            Swift = 4,
            //Immediate = 8,
            //Free = 16,
            DoubleMove = 32
        };

        public CombatActions combatActions;
    }
}
