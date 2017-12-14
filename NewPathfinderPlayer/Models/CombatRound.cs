using System;

namespace NewPathfinderPlayer.Models
{
    public class CombatRound
    {
        /// <summary>
        /// Flags to track which actions have been removed from the pool of actions for a single turn.
        /// TODO: Implement remaining action types.
        /// </summary>
        [Flags]
        public enum CombatActions
        {
            Standard = 1,
            Move = 2,
            // FullRound = 4,
            Swift = 8,
            // Immediate = 16,
            // Free = 32,
            DoubleMove = 64
        };

        public CombatActions combatActions;
    }
}