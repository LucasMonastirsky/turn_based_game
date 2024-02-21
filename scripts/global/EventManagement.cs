using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;

public class EventManager : EventManager<object> {
    public void Once (Action handler) {
        Once((object a) => handler());
    }

    public void Trigger () {
        Trigger(null);
    }
}

public class EventManager<T> {
    private TaskCompletionSource completion_source = new ();
    public Task Wait () {
        return completion_source.Task;
    }

    private List<Action<T>> once_handlers = new ();
    public void Once (Action<T> handler) {
        once_handlers.Add(handler);
    }

    private List<Predicate<T>> until_handlers = new ();
    public void Until (Predicate<T> handler) {
        until_handlers.Add(handler);
    }

    public List<Action<T>> always_handlers = new ();
    public void Always (Action<T> handler) {
        always_handlers.Add(handler);
    }

    public void Trigger (T arguments) {
        foreach (var handler in once_handlers) {
            handler(arguments);
        }

        foreach (var handler in always_handlers) { // TODO: maybe handlers should be tasks and awaited instead of added to a queue...
            handler(arguments);
        }

        foreach (var handler in until_handlers.ToList()) {
            if (handler(arguments)) {
                until_handlers.Remove(handler);
            }
        }

        completion_source.SetResult();
        completion_source = new ();

        once_handlers = new ();
    }

    public void Remove (Action<T> handler) {
        if (!once_handlers.Remove(handler)) always_handlers.Remove(handler);
    }
}

public delegate void EmptyDelegate ();