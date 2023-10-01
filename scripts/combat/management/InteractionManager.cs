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

        public static async Task ResolveQueue () {
            await Task.Delay(500);

            while(current.Queue.Count > 0) {
                await current.Queue.Dequeue()();
                await Task.Delay(500);
            }
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