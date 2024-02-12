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
    }
}