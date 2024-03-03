using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public class TurnManager {
        private static int turn_index = 0;
        
        public static CombatantStore Combatants => Battle.Combatants;
        public static Combatant ActiveCombatant => Combatants[turn_index];

        public static CombatAction CurrentAction { get; protected set; }

        public static string State = "Idle";
        private static bool IsPassQueued = false;

        public static async void BeginLoop () {
            while (true) {
                State = "Starting";
                Dev.Log(Dev.Tags.CombatManagement, $"Starting turn of {ActiveCombatant}");

                IsPassQueued = false;
                ActiveCombatant.OnTurnStart();

                while (ActiveCombatant.Tempo > 0 && !ActiveCombatant.IsDead && !IsPassQueued) {
                    State = "Requesting";
                    Dev.Log(Dev.Tags.CombatManagement, $"Requesting action from {ActiveCombatant}");

                    CombatantDisplayManager.Show();
                    CurrentAction = await ActiveCombatant.Controller.RequestAction();

                    if (CurrentAction is null) break;

                    State = "Resolving";
                    Dev.Log(Dev.Tags.CombatManagement, $"Starting action {CurrentAction}");

                    CombatantDisplayManager.Hide();
                    await InteractionManager.Act(CurrentAction);
                    CurrentAction = null;
                }

                State = "Ending";
                Dev.Log(Dev.Tags.CombatManagement, $"Ending turn of {ActiveCombatant}");

                CombatEvents.BeforeTurnEnd.Trigger();
                ActiveCombatant.StatusEffects.ToList().ForEach(effect => effect.Tick());
                await InteractionManager.ResolveQueue();
                await Timing.Delay();
                await InteractionManager.ResetCombatants();

                do {
                    turn_index++;
                    if (turn_index >= Combatants.Count) turn_index = 0;
                }
                while (Combatants[turn_index].IsDead);
            }
        }

        public static void PassTurn () {
            IsPassQueued = true;
        }
    }
}