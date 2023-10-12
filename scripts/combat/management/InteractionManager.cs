using CustomDebug;
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

        private static bool resolving;

        public static async Task ResolveQueue () {
            if (resolving) {
                Dev.Log($"Attempted to resolve while resolving");
                return;
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Resolving queue");

            resolving = true;

            await Timing.Delay();

            while(current.Queue.Count > 0) {
                await current.Queue.Dequeue()();
                await Timing.Delay();
            }

            resolving = false;
        }

        public static async void EndAction () {
            if (resolving) {
                Dev.Log($"Attempted to end action while resolving");
                return;
            }

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Triggering OnActionEnd events");

            foreach (var combatant in Battle.Combatants) {
                combatant.OnActionEnd();
            }

            await Timing.Delay();

            TurnManager.EndTurn();
        }
    }
}