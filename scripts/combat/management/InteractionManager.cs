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

            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, $"Starting action {action}");
            CombatantDisplayManager.Hide();

            action.User.Tempo -= action.TempoCost;

            await action.Run();

            await Timing.Delay();
            await ResetCombatants();

            foreach (var combatant in Battle.Combatants) {
                combatant.OnActionEnd();
            }

            action.Unbind();
            CombatantDisplayManager.Show();
        }

        public static async Task ResolveQueue () {
            Dev.Log(Dev.TAG.COMBAT_MANAGEMENT, "Resolving queue");

            while(current.Queue.Count > 0) {
                await Timing.Delay();
                await current.Queue.Dequeue()();
            }
        }

        public static async Task ResetCombatants () {
            List<Task> tasks = new ();

            foreach (var combatant in Battle.Combatants) {
                combatant.ResetAnimation();
                tasks.Add(combatant.ReturnToPosition());
            }

            await Task.WhenAll(tasks);
        }
    }
}