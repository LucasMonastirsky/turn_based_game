using Godot;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Combat {
    public partial class InteractionManager : Node {
        protected static InteractionManager current = new InteractionManager();

        public delegate Task StackEvent ();
        public StackEvent[] Stack = {};
        public static void AddStackEVent (StackEvent action) {
            current.Stack = current.Stack.Append(action).ToArray();
        }
        public static async Task ResolveStack () {
            foreach (var stack_event in current.Stack) {
                await stack_event();
            }

            current.Stack = new StackEvent[] {};
        }

        public delegate void OnActionEndEvent ();
        private OnActionEndEvent[] on_action_end_events = {};

        /// <summary>
        /// Use this to reset positions, animations, etc... Fire and forget. Do not use for mechanics.
        /// </summary>
        public static void OnActionEnd (OnActionEndEvent handler) {
            current.on_action_end_events = current.on_action_end_events.Append(handler).ToArray();
        }
        public static void EndAction () {
            foreach (var handler in current.on_action_end_events) {
                handler();
            }
            current.on_action_end_events = new OnActionEndEvent[] {};
        }
    }
}