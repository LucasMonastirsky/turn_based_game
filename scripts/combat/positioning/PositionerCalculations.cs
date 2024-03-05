using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class Positioner {
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

        #region Functions
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

        private static async Task recalculate_side (Side side) {
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

                    combatant.Position =  combatant.Position with { Slot = slots[i] };
                    GetSlotData(combatant.Position).Combatant = combatant;
                    
                    tasks.Add(combatant.DisplaceTo(GetWorldPosition(combatant.Position)));
                }
            }

            await Task.WhenAll(tasks);
        }
        #endregion
    }
}