using System;
using System.Collections.Generic;
using System.Linq;
using CustomDebug;
using Godot;

namespace Combat {
    [Tool] public partial class StandardPositioner : Node2D, IPositioner {
        private const int MAX_ROW_COUNT = 2; // if this changes, everything breaks
        private const int MAX_ROW_SIZE = 3;
        private const int ROW_SLOT_COUNT = MAX_ROW_SIZE * 2 - 1;

        #region Exports
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
        #endregion
    
        private Vector2[,] world_positions = new Vector2[MAX_ROW_COUNT, ROW_SLOT_COUNT];
        private List<ICombatant>[] left_combatants, right_combatants;
        private List<ICombatant> all_combatants => Battle.Combatants;

        private List<ICombatant>[] get_rows_on_side (Side side) {
            return side == Side.Left ? left_combatants : right_combatants;
        }

        public void Setup () {
            left_combatants = new List<ICombatant>[2]; // TODO: this is so dirty
            left_combatants[0] = Battle.Combatants.Where(combatant => combatant.CombatPosition.Side == Side.Left && combatant.CombatPosition.Row == 0).ToList();
            left_combatants[1] = Battle.Combatants.Where(combatant => combatant.CombatPosition.Side == Side.Left && combatant.CombatPosition.Row == 1).ToList();

            right_combatants = new List<ICombatant>[2];
            right_combatants[0] = Battle.Combatants.Where(combatant => combatant.CombatPosition.Side == Side.Right && combatant.CombatPosition.Row == 0).ToList();
            right_combatants[1] = Battle.Combatants.Where(combatant => combatant.CombatPosition.Side == Side.Right && combatant.CombatPosition.Row == 1).ToList();

            AdjustRowPositions(Side.Left, 0);
            AdjustRowPositions(Side.Right, 0);
            AdjustRowPositions(Side.Right, 1);

            foreach (var combatant in all_combatants) {
                combatant.UpdateWorldPos();
            }
        }

        private void calculate_both_sides () {
            for (var row_index = 0; row_index < MAX_ROW_COUNT; row_index++) {
                for (var row_pos_index = 0; row_pos_index < ROW_SLOT_COUNT; row_pos_index++) {
                    var x = center_distance + horizontal_distance * row_index;
                    var y = vertical_offset + vertical_distance * row_pos_index / ROW_SLOT_COUNT;
                    y -= vertical_distance / ROW_SLOT_COUNT * 2;

                    world_positions[row_index, row_pos_index] = new Vector2(x, y);
                }
            }

            QueueRedraw();
        }

        public Vector2 GetWorldPosition (CombatPosition position) {
            var pos = world_positions[position.Row, position.Slot];
            return pos with { X = pos.X * (int) position.Side };
        }

        public bool IsPositionAvailable (CombatPosition position) {
            var rows = get_rows_on_side(position.Side);

            var target_row = rows[position.Row];
            var other_row = rows[position.Row == 1 ? 0 : 1];

            if (target_row.Count >= MAX_ROW_SIZE || target_row.Count <= other_row.Count) {
                return false;
            }
            else {
                return true;
            }
        }

        public List<CombatPosition> GetAvailablePositions () {
            var available_positions = new List<CombatPosition>();

            for (var side_index = -1; side_index < 2; side_index += 2) {
                var side = (Side) side_index;

                for (var i = 0; i < MAX_ROW_COUNT; i++) {
                    var rows = get_rows_on_side(side);
                    var current_row = rows[i];
                    var other_row = rows[i == 1 ? 0 : 1];

                    
                }
            }

            return available_positions;
        }

        private void AdjustRowPositions (Side side, int row) {
            var combatants = Battle.Combatants.Where(combatant => combatant.CombatPosition.Side == side && combatant.CombatPosition.Row == row).ToList();

            int[] slots;

            switch (combatants.Count()) {
                case 1:
                    slots = new int[] { 2 };
                    break;
                case 2:
                    slots = new int[] { 1, 3 };
                    break;
                case 3:
                    slots = new int[] { 0, 2, 4 };
                    break;
                default:
                    slots = new int[] { };
                    break;
            }

            for (int i = 0; i < combatants.Count(); i++) {
                var combatant = combatants[i];
                combatant.CombatPosition = combatant.CombatPosition with { Slot = slots[i] };
                combatant.UpdateWorldPos();
            }
        }

        public override void _Draw () {
            for (var side = -1; side < 2; side += 2) {
                foreach (var pos in world_positions) {
                    DrawCircle(pos with { X = pos.X * side }, 7.5f, Colors.Red);
                }
            }
        }
    }
}