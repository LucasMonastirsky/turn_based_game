using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    [Tool] public partial class StandardPositioner : Node2D, IPositioner {
        private const int MAX_ROW_COUNT = 2;
        private const int MAX_ROW_SIZE = 5;

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
    
        private Vector2[,] world_positions = new Vector2[MAX_ROW_COUNT, MAX_ROW_SIZE];
        private List<ICombatant>[] left_combatants, right_combatants;
        private List<ICombatant> all_combatants => Battle.Combatants;

        public void Setup () {
            left_combatants = new List<ICombatant>[2]; // TODO: this is so dirty
            left_combatants[0] = Battle.Combatants.Where(combatant => combatant.Side == Side.Left && combatant.Row == 0).ToList();
            left_combatants[1] = Battle.Combatants.Where(combatant => combatant.Side == Side.Left && combatant.Row == 1).ToList();

            right_combatants = new List<ICombatant>[2];
            right_combatants[0] = Battle.Combatants.Where(combatant => combatant.Side == Side.Right && combatant.Row == 0).ToList();
            right_combatants[1] = Battle.Combatants.Where(combatant => combatant.Side == Side.Right && combatant.Row == 1).ToList();

            foreach (var combatant in all_combatants) {
                combatant.UpdateWorldPos();
            }
        }

        private void calculate_both_sides () {
            for (var row_index = 0; row_index < MAX_ROW_COUNT; row_index++) {
                for (var row_pos_index = 0; row_pos_index < MAX_ROW_SIZE; row_pos_index++) {
                    var x = center_distance + horizontal_distance * row_index;
                    var y = vertical_offset + vertical_distance * row_pos_index / MAX_ROW_SIZE;
                    y -= vertical_distance / MAX_ROW_SIZE * 2;

                    world_positions[row_index, row_pos_index] = new Vector2(x, y);
                }
            }

            QueueRedraw();
        }

        public Vector2 GetWorldPosition(Position position) {
            var rows = position.Side == Side.Left ? left_combatants : right_combatants;
            var row_size = rows[position.Row].Count;

            int[] possible_indexes;
            
            if (row_size == 3) possible_indexes = new int[] { 0, 2, 4 };
            else if (row_size == 2) possible_indexes = new int[] { 1, 3 };
            else possible_indexes = new int[] { 2 };

            var pos = world_positions[position.Row, possible_indexes[position.RowPos]];
            return pos with { X = pos.X * (int) position.Side };
        }

        public bool IsValidPosition(Position position)
        {
            throw new NotImplementedException();
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