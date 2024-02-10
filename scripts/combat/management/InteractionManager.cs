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

        private enum STATE { FREE, RESOLVING, ENDING, }
        private static STATE state = STATE.FREE;

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
            if (state == STATE.RESOLVING) {
                if (!ReactionsAllowed) {
                    Dev.Error("Started action while reactions were disallowed");
                }
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Starting action {CurrentAction.Name} ({CurrentAction.User.CombatName})");

            CombatantDisplayManager.Hide();
        }

        public static async Task ResolveQueue () {
            if (state == STATE.RESOLVING) {
                return;
            }

            if (state == STATE.ENDING) {
                Dev.Error($"InteractionManager: Trying to resolve during Ending state");
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Resolving queue");

            state = STATE.RESOLVING;

            await Timing.Delay();

            while(current.Queue.Count > 0) {
                await current.Queue.Dequeue()();
                await Timing.Delay();
            }

            while (ActionQueue.Count > 0) {
                var action = ActionQueue.Dequeue();
                if (action.Condition()) {
                    action.Run();
                    await Timing.Delay();
                }
                else {
                    Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Reaction {action.Name} from {action.User.CombatName} did not meet condition");
                }
            }

            state = STATE.FREE;
        }

        public static async Task EndAction () {
            if (state != STATE.FREE) return;

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnPreActionEnd events");

            foreach (var combatant in Battle.Combatants.All) {
                combatant.OnPreActionEnd();
            }

            await ResolveQueue();

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnActionEnd events");

            foreach (var combatant in Battle.Combatants) {
                combatant.OnActionEnd();
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Ending turn");

            ReactionsThisTurn = 0;
            TurnManager.EndTurn();
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