using Combat;
using Development;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class TargetingInterface : Node2D {
	private static TargetingInterface _current;
	private static TargetingInterface current {
		get {
			if (_current == null) {
                Dev.Error("No instance of TargetingInterface");
			}
			return _current;
		}
		set {
			_current = value;
		}
	}

	[Export] private PackedScene single_marker_scene;

	public override void _EnterTree() {
		current = this;
	}

	public static async Task<ICombatant> SelectSingleCombatant (List<ICombatant> combatants) {
		var markers = new Queue<KeyboardSelectionMarker>();
		var selection = new TaskCompletionSource<ICombatant>();

		foreach (var combatant in combatants) {
			var marker = current.single_marker_scene.Instantiate<KeyboardSelectionMarker>(); // TODO: I don't like setting public fields like this
			marker.Position = combatant.WorldPos;
			marker.OnCombatantSelected = () => {
				selection.TrySetResult(combatant);
			};
			current.AddChild(marker);
			markers.Enqueue(marker);
		}

		var result = await selection.Task;

		while (markers.Count > 0) {
			markers.Dequeue().QueueFree();
		}

		return result;
	}

	public static async Task<CombatPosition> SelectPosition (List<CombatPosition> positions) {
		var markers = new Queue<KeyboardSelectionMarker>();
		var selection = new TaskCompletionSource<CombatPosition>();

		foreach (var position in positions) {
			var marker = current.single_marker_scene.Instantiate<KeyboardSelectionMarker>();
			marker.Position = position.WorldPosition;
			marker.OnCombatantSelected = () => {
				selection.TrySetResult(position);
			};
			current.AddChild(marker);
			markers.Enqueue(marker);
		}

		var result = await selection.Task;

		while (markers.Count > 0) {
			markers.Dequeue().QueueFree();
		}

		return result;
	}
}
