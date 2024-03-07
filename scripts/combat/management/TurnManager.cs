using Development;

namespace Combat {
    public class TurnManager {
        private static int turn_index = 0;
        
        public static CombatantStore Combatants => Battle.Combatants;
        public static Combatant ActiveCombatant => Combatants[turn_index];

        public static CombatAction CurrentAction { get; protected set; }

        public static string State = "Idle";
        private static bool IsPassQueued = false;

        public static AttackResult LastAttack = null;

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

                    if (CurrentAction != null) {
                        State = "Resolving";
                        Dev.Log(Dev.Tags.CombatManagement, $"Starting action {CurrentAction}");

                        CombatantDisplayManager.Hide();
                        ActiveCombatant.Tempo -= CurrentAction.TempoCost;
                        await CurrentAction.Act();

                        if (LastAttack != null) {
                            if (LastAttack.AllowRiposte) {
                                await Timing.Delay();
                                await LastAttack.Defender.Riposte(LastAttack);
                            }

                            LastAttack = null;
                        }

                        await Timing.Delay();
                        await InteractionManager.ResetCombatants();
                        CurrentAction = null;
                    }
                }

                State = "Ending";
                Dev.Log(Dev.Tags.CombatManagement, $"Ending turn of {ActiveCombatant}");

                CombatEvents.BeforeTurnEnd.Trigger();
                ActiveCombatant.OnTurnEnd();

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