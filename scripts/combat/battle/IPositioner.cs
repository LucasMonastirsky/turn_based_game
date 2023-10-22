using System.Collections.Generic;
using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public struct CombatPosition {
        public Side Side;
        public int Row, RowPos;
    }

    public interface IPositioner {
        public void Setup ();
        public Vector2 GetWorldPosition (CombatPosition position);
        public bool IsPositionAvailable (CombatPosition position);
        public List<CombatPosition> GetAvailablePositions (Side side);
    }
}