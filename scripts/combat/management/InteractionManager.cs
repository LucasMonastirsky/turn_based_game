using Development;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class InteractionManager : Node {
        protected static InteractionManager current = new InteractionManager();

        public static CombatAction CurrentAction;

        public delegate Task QueueAction ();
        public Queue<QueueAction> Queue = new ();
        public static void AddQueueEvent (QueueAction action) {
            current.Queue.Enqueue(action);
        }

        public static Queue<CombatAction> ActionQueue = new ();

        public static int ReactionsThisTurn { get; private set; }
        public static bool ReactionsAllowed => ReactionsThisTurn < 1;

        public static async Task Act (CombatAction action) {
            if (!action.Bound) {
                Dev.Error($"Tried to act unbound action: {action.Name} ({action.User.CombatName})");
            }

            CurrentAction = action;

            await StartAction();
            await CurrentAction.Run();
            await EndAction();
            CurrentAction.Unbind();
        }

        public static async Task StartAction () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Starting action {CurrentAction.Name} ({CurrentAction.User.CombatName})");

            CombatantDisplayManager.Hide();
        }

        public static async Task ResolveQueue () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Resolving queue");

            await Timing.Delay();

            while(current.Queue.Count > 0) {
                await current.Queue.Dequeue()();
                await Timing.Delay();
            }

            while (ActionQueue.Count > 0) {
                var action = ActionQueue.Dequeue();

                if (action.IsAvailable() && action.Condition() && action.PassesSelectors()) {
                    action.Run();
                    await Timing.Delay();
                }
                else {
                    Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Reaction {action.Name} from {action.User.CombatName} is no longer available or does not meet condition");
                }
            }

            foreach (var combatant in Battle.Combatants) {
                combatant.ResetAnimation();
            }
        }

        public static async Task EndAction () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnPreActionEnd events");

            foreach (var combatant in Battle.Combatants.All) {
                combatant.OnPreActionEnd();
            }

            await ResolveQueue();

            foreach (var combatant in Battle.Combatants.All) {
                combatant.ReturnToPosition();
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnActionEnd events");

            foreach (var combatant in Battle.Combatants) {
                combatant.OnActionEnd();
            }

            await ResolveQueue();

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Ending action");

            ReactionsThisTurn = 0;
            TurnManager.EndTurn();
            await ResolveQueue();
            CombatantDisplayManager.Show();
        }

        public static async Task React (CombatAction action) {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"{action.User.CombatName} queued reaction {action.Name}");

            if (ReactionsAllowed) {
                ActionQueue.Enqueue(action);
            }

            ReactionsThisTurn++;
        }
    }
}