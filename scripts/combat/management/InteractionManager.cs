using CustomDebug;
using Godot;
using System.Collections.Generic;
using System.Linq;
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

            await Task.Delay(500);

            while(current.Queue.Count > 0) {
                await current.Queue.Dequeue()();
                await Task.Delay(500);
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

            await Task.Delay(500);

            TurnManager.EndTurn();
        }
    }
}