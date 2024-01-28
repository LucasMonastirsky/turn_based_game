using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    [Tool] public partial class Positioner : Node2D {
        private static Positioner current;

        private const int MAX_ROW_COUNT = 2; // if this changes, everything breaks
        private const int MAX_ROW_SIZE = 3; // this too
        private const int ROW_SLOT_COUNT = MAX_ROW_SIZE * 2 - 1;

        #region Exports
        protected int center_distance, vertical_distance, horizontal_distance, vertical_offset;
        [Export] protected int CenterDistance {
            get => center_distance;
            set {
                center_distance = value;
                calculate_positions();
            }
        }
        [Export] protected int VerticalDistance {
            get => vertical_distance;
            set {
                vertical_distance = value;
                calculate_positions();
            }
        }
        [Export] protected int HorizontalDistance {
            get => horizontal_distance;
            set {
                horizontal_distance = value;
                calculate_positions();
            }
        }
        [Export] protected int VerticalOffset {
            get => vertical_offset;
            set {
                vertical_offset = value;
                calculate_positions();
            }
        }
        #endregion
    
        public Dictionary<Side, List<RowData>> Rows;

        public class RowData {
            public List<SlotData> Slots;
            public int CombatantCount { get => Slots.Where(slot => slot.Combatant != null).Count(); }
            public int Index;

            public SlotData this[int index] => Slots[index];

            public RowData () {
                Slots = new ();
                for (int i = 0; i < ROW_SLOT_COUNT; i++) {
                    Slots.Add(new SlotData(this));
                }
            }
        }

        public class SlotData {
            public bool IsAvailable;
            public bool IsOccupied => Combatant != null;
            public Combatant Combatant;
            public Vector2 WorldPosition;
            public RowData Row { get; private set; }

            public SlotData (RowData row) {
                Row = row;
                Combatant = null;
                IsAvailable = true;
            }
        }

        public override void _Ready () {
            current = this;
        }

        public static void Setup () {
            foreach (var combatant in Battle.Combatants) {
                var pos = combatant.CombatPosition;
                combatant.MoveTo(GetWorldPosition(pos));
                current.Rows[pos.Side][pos.Row][pos.Slot].Combatant = combatant;
            }
        }

        public static Vector2 GetWorldPosition (CombatPosition position) {
            return current.Rows[position.Side][position.Row][position.Slot].WorldPosition;
        }

        public static List<CombatPosition> GetAvailablePositions () {
            var available_positions = new List<CombatPosition>();

            foreach (var side in new Side[] { Side.Left, Side.Right }) {
                for (int row = 0; row < MAX_ROW_COUNT; row++) {
                    for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                        if (current.Rows[side][row][slot].Combatant == null) {
                            available_positions.Add(new CombatPosition() { Side = side, Row = row, Slot = slot });
                        }
                    }
                }
            }

            return available_positions;
        }

        public static List<CombatPosition> GetMoveTargets (Combatant combatant) {
            var available_positions = new List<CombatPosition>();
            var position = combatant.CombatPosition;
            var side = position.Side;

            for (int row = 0; row < MAX_ROW_COUNT; row++) {
                for (int slot = 0; slot < ROW_SLOT_COUNT; slot++) {
                    var slot_data = current.Rows[side][row][slot];

                    if (slot_data.Combatant != null && slot_data.Combatant != combatant) {
                        available_positions.Add(new () { Side = side, Row = row, Slot = slot });
                    }
                    else if (
                        row != combatant.Row
                        && slot_data.Row.CombatantCount < MAX_ROW_SIZE
                        && current.Rows[position.Side][position.Row].CombatantCount > slot_data.Row.CombatantCount
                    ) {
                        if (
                            (slot < ROW_SLOT_COUNT - 2 && current.Rows[side][row][slot + 1].Combatant != null)
                            || (slot > 0 && current.Rows[side][row][slot - 1].Combatant != null)
                        ) {
                            available_positions.Add(new () { Side = side, Row = row, Slot = slot });
                        }
                    }
                }
            }

            return available_positions;
        }

        public static void SwitchPosition (Combatant combatant, CombatPosition target) {
            var old_pos = combatant.CombatPosition;

            combatant.CombatPosition = target;

            var new_slot = get_slot_data(target);
            if (new_slot.Combatant != null) {
                var new_combatant = new_slot.Combatant;
                new_combatant.CombatPosition = old_pos;
                new_slot.Combatant = combatant;
                get_slot_data(old_pos).Combatant = new_combatant;
            }
            else {
                
            }

            recalculate_side(target.Side);
        }
        
        private static SlotData get_slot_data (CombatPosition position) {
            return current.Rows[position.Side][position.Row][position.Slot];
        }

        private void calculate_positions () {
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

                Rows[Side.Left][row_index].Index = row_index;
                Rows[Side.Right][row_index].Index = row_index;
            }

            QueueRedraw();
        }

        private static void recalculate_side (Side side) {
            var tasks = new List<Task>();

            foreach (var row in current.Rows[side]) {
                foreach (var slot in row.Slots) {
                    slot.Combatant = null;
                }

                var combatants = Battle.Combatants.Where(combatant => (
                    combatant.Side == side
                    && combatant.Row == row.Index
                )).ToList();

                combatants.Sort((a, b) => a.Slot - b.Slot);

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

                for (var i = 0; i < combatants.Count; i++) {
                    var combatant = combatants[i];

                    combatant.CombatPosition =  combatant.CombatPosition with { Slot = slots[i] };
                    get_slot_data(combatant.CombatPosition).Combatant = combatant;
                    
                    tasks.Add(combatant.MoveTo(GetWorldPosition(combatant.CombatPosition)));
                }
            }
        }
    }
}