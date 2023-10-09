using Combat;
using CustomDebug;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class KeyboardController : Node2D, IController {
	[Export] private PackedScene SingleMarkerScene;

	public ICombatant hugo;

	public override void _Ready () {
		hugo = GetNode<StandardCombatant>("../Hugo");
		hugo.Controller = this;
	}

	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Test1")) {
			Dev.Log(Dev.TAG.INPUT, "Pressed Test1");

			_ = new Hugo.ActionStore.Swing(hugo).RequestTargetsAndRun();
		}
	}


	public async Task<ICombatant> RequestSingleTarget (ICombatant user, TargetSelector selector) {
		var selectables = Battle.Current.Combatants;
		if (selector.Side != null) {
			selectables = Battle.Current.Combatants.Where(
				x => (int) x.Side * (int) selector.Side == (int) user.Side
			).ToList();
		}

		var markers = new Queue<KeyboardSelectionMarker>();
		var selection = new TaskCompletionSource<ICombatant>();
		foreach (var combatant in selectables) {
			var marker = SingleMarkerScene.Instantiate<KeyboardSelectionMarker>(); // TODO: I don't like setting public fields like this
			marker.Position = combatant.WorldPos;
			marker.OnCombatantSelected = () => {
				selection.TrySetResult(combatant);
			};
			AddChild(marker);
			markers.Enqueue(marker);
		}

		var result = await selection.Task;

		while (markers.Count > 0) {
			markers.Dequeue().QueueFree();
		}

		return result;
	}
}
