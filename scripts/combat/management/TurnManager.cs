using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public class TurnManager {
        private static int turn_index = 0;
        
        public static List<Combatant> Combatants;
        public static Combatant ActiveCombatant => Combatants[turn_index];

        public static CombatAction CurrentAction { get; protected set; }

        public static string State = "Idle";
        private static bool IsPassQueued = false;

        public static async void BeginLoop () {
            Combatants = Battle.Combatants.All;

            while (true) { // TODO: add condition for ending turn loop
                State = "Starting";

                IsPassQueued = false;
                ActiveCombatant.OnTurnStart();

                while (ActiveCombatant.Tempo > 0 && !ActiveCombatant.IsDead && !IsPassQueued) {
                    State = "Requesting";

                    CurrentAction = await ActiveCombatant.Controller.RequestAction();

                    State = "Resolving";

                    await InteractionManager.Act(CurrentAction);
                    CurrentAction = null;
                }

                State = "Ending";

                CombatEvents.BeforeTurnEnd.Trigger();
                await InteractionManager.ResolveQueue();

                foreach (var combatant in Combatants.ToList()) {
                    if (combatant.IsDead) Combatants.Remove(combatant);
                }

                turn_index++;
                if (turn_index >= Combatants.Count) turn_index = 0;
            }
        }

        public static void PassTurn () {
            IsPassQueued = true;
        }
    }
}