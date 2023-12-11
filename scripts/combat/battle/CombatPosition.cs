using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public struct CombatPosition {
        public Side Side;
        public int Row, Slot;

        public Vector2 WorldPosition => Positioner.GetWorldPosition(this);
    }
}