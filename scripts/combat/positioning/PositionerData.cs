using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
    public partial class Positioner {
        public Dictionary<Side, List<RowData>> Rows;

        public class RowData {
            public List<SlotData> Slots;
            public int CombatantCount { get => Slots.Where(slot => slot.Combatant != null).Count(); }
            public int Index;
            public Side Side;

            public SlotData this[int index] => Slots[index];

            public RowData (Side side) {
                Side = side;
                Slots = new ();
                for (int i = 0; i < ROW_SLOT_COUNT; i++) {
                    Slots.Add(new SlotData(this, i));
                }
            }
        }

        public class SlotData {
            public bool IsAvailable;
            public bool IsOccupied => Combatant != null;
            public int Index { get; init; }
            public Combatant Combatant;
            public Vector2 WorldPosition;
            public RowData Row { get; private set; }
            public CombatPosition Position => new CombatPosition () { Side = Row.Side, Row = Row.Index, Slot = Index, };

            public SlotData (RowData row, int index) {
                Row = row;
                Combatant = null;
                IsAvailable = true;
                Index = index;
            }
        }
    }
}