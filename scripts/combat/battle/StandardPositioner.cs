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
    
        public Dictionary<Side, List<RowData>> Rows;

        public class RowData {
            public List<SlotData> Slots;
            public int CombatantCount;

            public SlotData this[int index] => Slots[index];

            public RowData () {
                Slots = new ();
                for (int i = 0; i < ROW_SLOT_COUNT; i++) {
                    Slots.Add(new SlotData());
                }
            }
        }

        public class SlotData {
            public bool IsAvailable;
            public bool IsOccupied => Combatant != null;
            public ICombatant Combatant;
            public Vector2 WorldPosition;

            public SlotData () {
                Combatant = null;
                IsAvailable = true;
            }
        }

        public void Setup () {
            foreach (var combatant in Battle.Combatants) {
                combatant.UpdateWorldPos();
                var pos = combatant.CombatPosition;
                Rows[pos.Side][pos.Row][pos.Slot].Combatant = combatant;
            }
        }

        private void calculate_both_sides () {
            Rows = new () {
                { Side.Left, new List<RowData>() },
                { Side.Right, new List<RowData>() },
            };

            for (var row_index = 0; row_index < MAX_ROW_COUNT; row_index++) {
                Rows[Side.Left].Add(new RowData());
                Rows[Side.Right].Add(new RowData());

                for (var slot_index = 0; slot_index < ROW_SLOT_COUNT; slot_index++) {
                    var x = center_distance + horizontal_distance * row_index;
                    var y = vertical_offset + vertical_distance * slot_index / ROW_SLOT_COUNT;
                    y -= vertical_distance / ROW_SLOT_COUNT * 2;

                    Rows[Side.Left][row_index][slot_index].WorldPosition = new Vector2(-x, y);
                    Rows[Side.Right][row_index][slot_index].WorldPosition = new Vector2(x, y);
                }
            }

            QueueRedraw();
        }

        public Vector2 GetWorldPosition (CombatPosition position) {
            return Rows[position.Side][position.Row][position.Slot].WorldPosition;
        }

        public List<CombatPosition> GetAvailablePositions () {
            var available_positions = new List<CombatPosition>();

            foreach (var side in new Side[] { Side.Left, Side.Right }) {
                for (int row = 0; row < MAX_ROW_COUNT; row++) {
                    for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                        if (Rows[side][row][slot].Combatant == null) {
                            available_positions.Add(new CombatPosition() { Side = side, Row = row, Slot = slot });
                        }
                    }
                }
            }

            return available_positions;
        }

        public List<CombatPosition> GetMoveTargets (ICombatant combatant) {
            var available_positions = new List<CombatPosition>();

            var side = combatant.CombatPosition.Side;

            for (int row = 0; row < MAX_ROW_COUNT; row++) {
                for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                    if (Rows[side][row][slot].Combatant != null && Rows[side][row][slot].Combatant != combatant) {
                        available_positions.Add(new () { Side = side, Row = row, Slot = slot });
                    }
                    else if (row != combatant.CombatPosition.Row) {
                        if (
                            (slot < ROW_SLOT_COUNT - 2 && Rows[side][row][slot + 1].Combatant != null)
                            || (slot > 0 && Rows[side][row][slot - 1].Combatant != null)
                        ) {
                            available_positions.Add(new () { Side = side, Row = row, Slot = slot });
                        }
                    }
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
            foreach (Side side in Rows.Keys) {
                foreach (RowData row in Rows[side]) {
                    foreach (SlotData slot in row.Slots) {
                        DrawCircle(slot.WorldPosition, 5, Colors.Red);
                    }
                }
            }
        }
    }
}