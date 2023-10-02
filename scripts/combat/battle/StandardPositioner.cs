using System;
using CustomDebug;
using Godot;

namespace Combat {
    [Tool] public partial class StandardPositioner : Node2D, IPositioner {
        [Export] private bool Calculate { get => false; set { calculate_both_sides(); } }
        [Export] protected int LeftRowCount, LeftRowSize, RightRowCount, RightRowSize;

        protected int center_distance, vertical_distance, horizontal_distance, vertical_offset;
        [Export] protected int CenterDistance {
            get => center_distance;
            set {
                center_distance = value;
                calculate_both_sides();
            }
        }
        [Export] protected int VerticalDistance {
            get => vertical_distance;
            set {
                vertical_distance = value;
                calculate_both_sides();
            }
        }
        [Export] protected int HorizontalDistance {
            get => horizontal_distance;
            set {
                horizontal_distance = value;
                calculate_both_sides();
            }
        }
        [Export] protected int VerticalOffset {
            get => vertical_offset;
            set {
                vertical_offset = value;
                calculate_both_sides();
            }
        }

        private Vector2[,] left_positions, right_positions;

        public Vector2 GetWorldPosition(Position position) {
            return (position.Side == Side.Left ? left_positions : right_positions)[position.Row, position.RowPos];
        }

        public bool IsValidPosition(Position position)
        {
            throw new System.NotImplementedException();
        }

        private Vector2[,] calculate_side_positions (int side, int row_count, int row_size) {
            var result = new Vector2[row_count, row_size];

            for (var r = 0; r < row_count; r++) {
                for (var p = 0; p < row_size; p++) {
                    result[r, p] = new Vector2(
                        side * (CenterDistance + (r * HorizontalDistance)),
                        VerticalOffset + (p * vertical_distance - (row_size / 2 * vertical_distance)) + (0.5f - (float) row_size / 2 % 1) * vertical_distance  
                    );
                }
            }

            return result;
        }

        private void calculate_both_sides () {
            left_positions = calculate_side_positions(-1, LeftRowCount, LeftRowSize);
            right_positions = calculate_side_positions(1, RightRowCount, RightRowSize);
            QueueRedraw();
            var s = "";
            foreach (var pos in left_positions) {
                s += $"({pos.X},{pos.Y})";
            }
            s += "\n";
            foreach (var pos in right_positions) {
                s += $"({pos.X},{pos.Y})";
            }
            s += "\n";
            Dev.Log(s);
        }

        public override void _Ready () {
            calculate_both_sides();
        }

        public override void _Draw () {
            void DrawPos (Vector2 pos) {
                DrawCircle(pos, 1.0f, Colors.Beige);
            }

            foreach (var pos in right_positions) {
                DrawPos(pos);
            }

            foreach (var pos in left_positions) {
                DrawPos(pos);
            }
        }
    }
}