using Combat;
using Godot;

public partial class AsyncInput : Node {
	public static EventManager Continue = new ();
	public static EventManager Cancel = new ();

    public override void _Input (InputEvent @event) {
		if (@event.IsActionPressed("Continue")) {
			Continue.Trigger();
		}
		if (@event.IsActionPressed("Cancel")) {
			Cancel.Trigger();
		}
	}
}
