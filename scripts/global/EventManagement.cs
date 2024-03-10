using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EventManager : EventManager<object> {
    public void Once (Func<Task> handler) {
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

    private List<Func<T, Task>> once_handlers = new ();
    public void Once (Func<T, Task> handler) {
        once_handlers.Add(handler);
    }

    private List<Func<T, Task<bool>>> until_handlers = new ();
    public void Until (Func<T, Task<bool>> handler) {
        until_handlers.Add(handler);
    }

    public List<Func<T, Task>> always_handlers = new ();
    public void Always (Func<T, Task> handler) {
        always_handlers.Add(handler);
    }

    public async Task Trigger (T arguments) {
        foreach (var handler in once_handlers) {
            await handler(arguments);
        }

        foreach (var handler in always_handlers) { // TODO: maybe handlers should be tasks and awaited instead of added to a queue...
            await handler(arguments);
        }

        foreach (var handler in until_handlers.ToList()) {
            if (await handler(arguments)) {
                until_handlers.Remove(handler);
            }
        }

        completion_source.SetResult();
        completion_source = new ();

        once_handlers = new ();
    }

    public void Remove (Func<T, Task> handler) {
        if (!once_handlers.Remove(handler)) always_handlers.Remove(handler);
    }
}

public delegate void EmptyDelegate ();