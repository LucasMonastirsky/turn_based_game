using Godot;
using System.Collections.Generic;

public partial class AsyncInput : Node {
	private static AsyncInput current;

	public class Manager {
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

	public static Manager Cancel = new ();

    public override void _Ready() {
        current = this;
    }

    public override void _Input (InputEvent @event) {
		if (@event.IsActionPressed("Cancel")) {
			Cancel.Trigger();
		}
	}
}
