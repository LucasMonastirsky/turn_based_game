using Development;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class InteractionManager : Node {
        protected static InteractionManager current = new InteractionManager();

        public delegate Task QueueAction ();
        public Queue<QueueAction> Queue = new ();
        public static void AddQueueEvent (QueueAction action) {
            current.Queue.Enqueue(action);
        }

        private enum STATE { FREE, RESOLVING, ENDING, }
        private static STATE state = STATE.FREE;

        public static async Task StartAction () {
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

            state = STATE.FREE;
        }

        public static async Task EndAction () {
            if (state != STATE.FREE) return;

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnPreActionEnd events");

            foreach (var combatant in Battle.Combatants.All) {
                combatant.OnPreActionEnd();
            }

            if (current.Queue.Count > 0) await ResolveQueue();

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnActionEnd events");

            foreach (var combatant in Battle.Combatants) {
                combatant.OnActionEnd();
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Ending turn");

            TurnManager.EndTurn();
            CombatantDisplayManager.Show();
        }
    }
}