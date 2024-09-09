using System;

namespace Combat {
    public record RollResult {
        public Combatant Combatant;
        public Stat Stat;

        public int Total = 0;
        public int DiceValue = 0;
        public int Bonus = 0;

        public static RollResult AutoFail => new () {
            Total = int.MinValue,
        };
    }
}