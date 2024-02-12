using System;

namespace Combat {
    public class CombatTarget : Targetable {
        public CombatPosition Position;
        public Combatant Combatant => Positioner.GetSlotData(Position).Combatant;

        public Side Side => Position.Side;
        public int Row => Position.Row;
        public int Slot => Position.Slot;

        public CombatTarget (CombatPosition position) {
            Position = position;
        }

        public CombatTarget (Combatant combatant) {
            Position = combatant.CombatPosition;
        }

        public CombatTarget ToTarget () => this;
    }
}