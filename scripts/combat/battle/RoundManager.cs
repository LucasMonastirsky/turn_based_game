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

        public static List<RoundItem> CurrentRound, NextRound;

        public static Combatant ActiveCombatant => CurrentRound.First().Combatant;

        public static Combatant GetCombatant (int index) {
            if (index < CurrentRound.Count) return CurrentRound[index].Combatant;
            else {
                var new_index = index - CurrentRound.Count;
                if (new_index < NextRound.Count) return NextRound[new_index].Combatant; 
            }

            return null;
        }

        public static void Begin () {
            var items = Battle.Combatants.All.Select(combatant => new RoundItem (combatant));

            CurrentRound = new (items.OrderBy(item => item.Priority));
            NextRound = new ();

            RoundDisplay.UpdateIcons();
        }

        public static void Next () {
            var new_item = new RoundItem (CurrentRound[0].Combatant);
            CurrentRound.RemoveAt(0);

            NextRound = NextRound.Append(new_item).OrderBy(item => item.Priority).ToList();

            if (CurrentRound.Count < 1) {
                CurrentRound = NextRound;
                NextRound = new ();
            }

            RoundDisplay.UpdateIcons();
        }

        public static void Remove (List<Combatant> combatants) {
            CurrentRound.RemoveAll(item => combatants.Contains(item.Combatant));
            NextRound.RemoveAll(item => combatants.Contains(item.Combatant));

            if (CurrentRound.Count < 1) {
                CurrentRound = NextRound;
                NextRound = new ();
            }
        }
    }
}