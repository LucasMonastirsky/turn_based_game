using System.Collections;
using System.Collections.Generic;

namespace Combat {
    public class CombatantStore : IEnumerable<Combatant> {
        private List<Combatant> combatants;

        public CombatantStore (List<Combatant> combatants) {
            this.combatants = combatants;
        }

        public int Count => combatants.Count;

        public List<Combatant> ToList () {
            return combatants;
        }

        public Combatant this [int index] {
            get => combatants[index];
            set => combatants[index] = value;
        }

        public List<Combatant> All => combatants;
        public CombatantStore Alive => new (combatants.FindAll(combatant => !combatant.IsDead));
        public CombatantStore Dead => new (combatants.FindAll(combatant => combatant.IsDead));

        public CombatantStore OnOppositeSide (Side side) {
            return new (combatants.FindAll(combatant => combatant.Side != side));
        }

        public CombatantStore OnSide (Side side) {
            return new (combatants.FindAll(combatant => combatant.Side == side));
        }

        public CombatantStore OnRow (int row) {
            return new (combatants.FindAll(combatant => combatant.Row == row));
        }

        public CombatantStore OnSlot (int slot) {
            return new (combatants.FindAll(combatant => combatant.Slot == slot));
        }

        public IEnumerator<Combatant> GetEnumerator() {
            return combatants.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return combatants.GetEnumerator();
        }
    }
}