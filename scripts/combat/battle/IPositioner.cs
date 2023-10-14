using Godot;

namespace Combat {
    public enum Side { Left = -1, Right = 1, }

    public struct Position {
        public Side Side;
        public int Row, RowPos;
    }

    public interface IPositioner {
        public void Setup ();
        public Vector2 GetWorldPosition (Position position);
        public bool IsValidPosition (Position position);
    }
}