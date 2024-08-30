using System;

namespace Combat {
    public class Target : Targetable {
        public CombatPosition Position;
        public Combatant Combatant => Positioner.GetSlotData(Position).Combatant;
        public bool IsEmpty => Combatant == null;

        public Side Side => Position.Side;
        public int Row => Position.Row;
        public int Slot => Position.Slot;

        public Target (CombatPosition position) {
            Position = position;
        }

        public Target (Combatant combatant) {
            Position = combatant.Position;
        }

        public Target ToTarget () => this;

        public int VerticalDistanceTo (Target target) {
            return Math.Abs(Position.Slot - target.Position.Slot);
        }
    }
}