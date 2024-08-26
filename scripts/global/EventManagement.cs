using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EventManager : EventManager<object> {
    public void Once (Func<Task> handler) {
        Once((object a) => handler());
    }

    public async Task Trigger () {
        await Trigger(null);
    }
}

public class EventManager<T> {
    private bool resolving = false;
    private List<Func<T, Task>> removal_queue = new ();

    private TaskCompletionSource completion_source = new ();
    public Task Wait () {
        return completion_source.Task;
    }

    private List<Func<T, Task>> once_handlers = new ();
    public void Once (Func<T, Task> handler) {
        once_handlers.Add(handler);
    }

    public List<Func<T, Task>> always_handlers = new ();
    public void Always (Func<T, Task> handler) {
        always_handlers.Add(handler);
    }

    public async Task<T> Trigger (T arguments) {
        resolving = true;

        foreach (var handler in once_handlers) {
            await handler(arguments);
        }

        foreach (var handler in always_handlers) {
            if (!removal_queue.Contains(handler)) await handler(arguments);
        }

        completion_source.SetResult();
        completion_source = new ();

        once_handlers = new ();

        resolving = false;

        foreach (var handler in removal_queue) {
            Remove(handler);
        }

        return arguments;
    }

    public void Remove (Func<T, Task> handler) {
        if (resolving) removal_queue.Add(handler);
        else if (!once_handlers.Remove(handler)) always_handlers.Remove(handler);
    }

    public static EventManager<T> operator + (EventManager<T> event_manager, Func<T, Task> handler) {
        event_manager.Always(handler);
        return event_manager;
    }

    public static EventManager<T> operator - (EventManager<T> event_manager, Func<T, Task> handler) {
        event_manager.Remove(handler);
        return event_manager;
    }
}

public delegate void EmptyDelegate ();