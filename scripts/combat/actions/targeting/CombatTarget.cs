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
            Position = combatant.Position;
        }

        public CombatTarget ToTarget () => this;

        public int VerticalDistanceTo (CombatTarget target) {
            return Math.Abs(Position.Slot - target.Position.Slot);
        }
    }
}