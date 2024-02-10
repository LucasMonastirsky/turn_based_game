using Combat;
using Godot;

public partial class AsyncInput : Node {
	public static EventManager<EmptyDelegate> Cancel = new ();

    public override void _Input (InputEvent @event) {
		if (@event.IsActionPressed("Cancel")) {
			Cancel.Trigger();
		}
	}
}
