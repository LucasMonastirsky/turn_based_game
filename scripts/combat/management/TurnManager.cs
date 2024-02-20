using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public class TurnManager {
        private static int turn_index = 0;
        
        public static Combatant ActiveCombatant => Battle.Combatants.Alive[turn_index];

        public static string State = "Idle";
        private static bool IsPassQueued = false;

        public static async void BeginLoop () {
            while (true) { // TODO: add condition for ending turn loop
                State = "Starting";

                IsPassQueued = false;
                ActiveCombatant.OnTurnstart();

                while (ActiveCombatant.Tempo > 0 && !IsPassQueued) {
                    State = "Requesting";

                    var action = await ActiveCombatant.Controller.RequestAction();
                    
                    State = "Resolving";

                    await InteractionManager.Act(action);
                }

                State = "Ending";

                CombatEvents.BeforeTurnEnd.Trigger();

                turn_index++;
                if (turn_index >= Battle.Combatants.Alive.Count) turn_index = 0;
            }
        }

        public static void PassTurn () {
            IsPassQueued = true;
        }
    }
}