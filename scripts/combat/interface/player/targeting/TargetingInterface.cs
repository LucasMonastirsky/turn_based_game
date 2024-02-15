using Combat;
using Development;
using Godot;
using System.Collections.Generic;
using System.Linq;
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

	public static async Task<CombatTarget> SelectSingleCombatant (List<CombatTarget> targets) {
		CombatPlayerInterface.HideActionList();

		var cancel = new TaskCompletionSource();
		AsyncInput.Cancel.Once(() => {
			cancel.SetResult();
		});

		var markers = new Queue<SelectionMarker>();
		var selection = new TaskCompletionSource<CombatTarget>();

		foreach (var combatant in targets.Select(target => target.Combatant)) {
			var marker = current.single_marker_scene.Instantiate<SelectionMarker>(); // TODO: I don't like setting public fields like this
			marker.Position = combatant.WorldPos;
			marker.OnCombatantSelected = () => {
				selection.TrySetResult(new CombatTarget(combatant));
			};
			current.AddChild(marker);
			markers.Enqueue(marker);
		}

		var first_resolved_task = await Task.WhenAny(cancel.Task, selection.Task);

		while (markers.Count > 0) {
			markers.Dequeue().QueueFree();
		}

		if (first_resolved_task == selection.Task) {
			return selection.Task.Result;
		}
		else {
			return null;
		}
	}

	public static async Task<CombatTarget> SelectPosition (List<CombatTarget> targets) {
		var cancel = new TaskCompletionSource();
		AsyncInput.Cancel.Once(() => {
			cancel.SetResult();
		});

		var markers = new Queue<SelectionMarker>();
		var selection = new TaskCompletionSource<CombatTarget>();

		foreach (var target in targets) {
			var marker = current.single_marker_scene.Instantiate<SelectionMarker>();
			marker.Position = target.Position.WorldPosition;
			marker.OnCombatantSelected = () => {
				selection.TrySetResult(target);
			};
			current.AddChild(marker);
			markers.Enqueue(marker);
		}

		var first_resolved_task = await Task.WhenAny(cancel.Task, selection.Task);

		while (markers.Count > 0) {
			markers.Dequeue().QueueFree();
		}

		if (first_resolved_task == selection.Task) {
			return selection.Task.Result;
		}
		else {
			return null;
		}
	}
}
