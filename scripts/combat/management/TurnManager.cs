using System.Collections.Generic;
using System.Linq;
using Development;

namespace Combat {
    public class TurnManager {
        private static int turn_index = 0;
        
        public static CombatantStore Combatants;
        public static Combatant ActiveCombatant => Combatants[turn_index];

        public static CombatAction CurrentAction { get; protected set; }

        public static string State = "Idle";
        private static bool IsPassQueued = false;

        public static async void BeginLoop () {
            while (true) {
                Combatants = Battle.Combatants.Alive;

                State = "Starting";
                Dev.Log(Dev.Tags.CombatManagement, $"Starting turn of {ActiveCombatant}");

                IsPassQueued = false;
                ActiveCombatant.OnTurnStart();

                while (ActiveCombatant.Tempo > 0 && !ActiveCombatant.IsDead && !IsPassQueued) {
                    State = "Requesting";
                    Dev.Log(Dev.Tags.CombatManagement, $"Requesting action from {ActiveCombatant}");

                    CurrentAction = await ActiveCombatant.Controller.RequestAction();

                    if (CurrentAction is null) break;

                    State = "Resolving";
                    Dev.Log(Dev.Tags.CombatManagement, $"Starting action {CurrentAction}");

                    await InteractionManager.Act(CurrentAction);
                    CurrentAction = null;
                }

                State = "Ending";
                Dev.Log(Dev.Tags.CombatManagement, $"Ending turn of {ActiveCombatant}");

                CombatEvents.BeforeTurnEnd.Trigger();
                await InteractionManager.ResolveQueue();

                turn_index++;
                if (turn_index >= Combatants.Count) turn_index = 0;
            }
        }

        public static void PassTurn () {
            IsPassQueued = true;
        }
    }
}