using System.Collections.Generic;
using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public struct CombatPosition : Targetable {
        public Side Side;
        public int Row, Slot;

        public Vector2 WorldPosition => Positioner.GetWorldPosition(this);

        public CombatTarget ToTarget () => new CombatTarget (this);

        public Combatant Combatant => Positioner.GetSlotData(this).Combatant;

        public CombatPosition OppositeSide => this with { Side = (Side) ((int) Side * -1) };

        public List<CombatPosition> Neighbours {
            get {
                var result = new List<CombatPosition> ();

                if (Slot > 0) result.Add(this with { Slot = Slot - 1 });
                if (Slot < 4) result.Add(this with { Slot = Slot + 1 });

                return result;
            }
        }
        public override string ToString () {
            return $"{(Side == Side.Left ? "L" : "R")}{(Row == 0 ? "F" : "R")}{Slot}"; 
        }
    }
}