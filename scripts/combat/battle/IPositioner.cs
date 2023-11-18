using System.Collections.Generic;
using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public struct CombatPosition {
        public Side Side;
        public int Row, Slot;

        public Vector2 WorldPosition => Battle.Positioner.GetWorldPosition(this);
    }

    public interface IPositioner {
        public void Setup ();
        public Vector2 GetWorldPosition (CombatPosition position);
        public List<CombatPosition> GetAvailablePositions ();
        public List<CombatPosition> GetMoveTargets (ICombatant combatant);
    }
}