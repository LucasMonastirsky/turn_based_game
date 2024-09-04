using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public static class RoundManager {
        public class RoundItem {
            public Combatant Combatant;
            public int Priority;

            public RoundItem (Combatant combatant) {
                Combatant = combatant;
                Priority = combatant.Roll(Dice.D10, Stat.Initiative);
            }
        }

        public static Queue<RoundItem> CurrentRound, NextRound;

        public static Combatant ActiveCombatant => CurrentRound.First().Combatant;

        public static void Begin () {
            var items = Battle.Combatants.All.Select(combatant => new RoundItem (combatant));

            CurrentRound = new (items.OrderBy(item => item.Priority));
            NextRound = new ();
        }

        public static void Next () {
            var combatant = CurrentRound.Dequeue().Combatant;
            var new_item = new RoundItem (combatant);

            NextRound.Enqueue(new_item);
            NextRound.OrderBy(item => item.Priority);

            if (CurrentRound.Count < 1) {
                CurrentRound = NextRound;
                NextRound = new ();
            }
        }
    }
}