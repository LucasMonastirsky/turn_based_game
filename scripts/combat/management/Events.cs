using System.Collections.Generic;

namespace Combat {
    public class EventManager<Delegate> {
        public delegate void Handler ();
        private List<Handler> handlers = new ();

        public void Once (Handler handler) {
            handlers.Add(handler);
        }

        public void Trigger () {
            foreach (var handler in handlers) {
                handler();
            }

            handlers = new ();
        }
    }

    public delegate void EmptyDelegate ();

    public static class Events {
        public static EventManager<EmptyDelegate> BeforeAttack = new ();
    }    
}