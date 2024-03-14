using System.Collections.Generic;
using Godot;

namespace Combat {
    public class Side {
        public static Side Left => new Side(-1);
        public static Side Right => new Side(1);

        public int Value { get; init; }
        public Side Opposite => Value == -1 ? Right : Left;

        public Side (int value) {
            Value = value;
        }

        public override string ToString () => Value == -1 ? "Left" : "Right";

        public override bool Equals (object other_side) {
            if (other_side.GetType() == typeof(int)) return (int) other_side == Value;
            else if (other_side.GetType() == typeof(Side)) return ((Side) other_side).Value == Value;
            else return false;
        }

        public static bool operator == (Side a, Side b) {
            return a.Equals(b);
        }

        public static bool operator != (Side a, Side b) {
            return !a.Equals(b);
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }

    public struct CombatPosition : Targetable {
        public Side Side;
        public int Row, Slot;

        public Vector2 WorldPosition => Positioner.GetWorldPosition(this);

        public CombatTarget ToTarget () => new CombatTarget (this);

        public Combatant Combatant => Positioner.GetSlotData(this).Combatant;

        public CombatPosition OppositeSide => this with { Side = Side.Opposite };

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