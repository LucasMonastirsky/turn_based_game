using System.Linq;
using Development;

namespace Combat {
    public class TurnManager {
        public static CombatantStore Combatants => Battle.Combatants;
        public static Combatant ActiveCombatant { get; private set; }

        public static CombatAction CurrentAction { get; protected set; }

        public enum TurnState {
            Idle, Starting, Requesting, Resolving, Ending,
        }
        public static TurnState State = TurnState.Idle;
        private static bool IsPassQueued = false;

        public static AttackResult LastAttack = null;

        public static async void BeginLoop () {
            while (true) {
                if (Combatants.OnSide(Side.Right).ToList().All(combatant => combatant.IsDead)) {
                    Battle.Current.End();
                    break;
                }

                ActiveCombatant = RoundManager.ActiveCombatant;
                CombatantDetail.Combatant = ActiveCombatant;

                State = TurnState.Starting;
                Dev.Log(Dev.Tags.CombatManagement, $"Starting turn of {ActiveCombatant}");

                IsPassQueued = false;
                ActiveCombatant.OnTurnStart();

                while (!ActiveCombatant.IsDead) {
                    if (ActiveCombatant.Tempo < 1 || IsPassQueued) {
                        break;
                    }

                    State = TurnState.Requesting;
                    Dev.Log(Dev.Tags.CombatManagement, $"Requesting action from {ActiveCombatant}");

                    CombatantDisplayManager.Show();
                    CurrentAction = await ActiveCombatant.Controller.RequestAction();
                    ActionDisplay.HideActionList();

                    if (CurrentAction != null) {
                        if (!CurrentAction.PassesSelectors()) Dev.Error($"{ActiveCombatant}.{CurrentAction} does not pass selectors");

                        await CombatEvents.BeforeAction.Trigger(CurrentAction);

                        State = TurnState.Resolving;
                        Dev.Log(Dev.Tags.CombatManagement, $"Starting action {CurrentAction}");

                        CombatantDisplayManager.Hide();
                        ActiveCombatant.Tempo -= CurrentAction.TempoCost;
                        if (!ActiveCombatant.IsDead) await CurrentAction.Act();

                        if (!IsPassQueued) await Timing.Delay();

                        if (LastAttack != null) {
                            if (LastAttack.AllowRiposte && !LastAttack.Defender.IsDead && !LastAttack.Defender.HasStatusEffect<Stunned>()) {
                                var riposte = LastAttack.Defender.GetRiposte(LastAttack);

                                if (riposte != null) {
                                    await riposte.Act();
                                    await Timing.Delay();
                                }
                            }

                            LastAttack = null;
                        }

                        await InteractionManager.ResolveQueue();

                        await InteractionManager.ResetCombatants();
                        await CombatEvents.AfterAction.Trigger(CurrentAction);
                        await InteractionManager.ResetCombatants();

                        CurrentAction.Unbind();
                        CurrentAction = null;
                    }

                    else IsPassQueued = true;
                }

                State = TurnState.Ending;
                Dev.Log(Dev.Tags.CombatManagement, $"Ending turn of {ActiveCombatant}");

                await CombatEvents.BeforeTurnEnd.Trigger(ActiveCombatant);
                ActiveCombatant.OnTurnEnd();

                await InteractionManager.ResolveQueue();
                if (!IsPassQueued) await Timing.Delay();
                await InteractionManager.ResetCombatants();

                RoundManager.Next();
            }
        }

        public static void PassTurn () {
            IsPassQueued = true;
        }
    }
}