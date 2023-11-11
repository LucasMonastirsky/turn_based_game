using Combat;
using CustomDebug;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class KeyboardController : Controller {
	[Export] private PackedScene SingleMarkerScene;

	private CombatPlayerInterface player_interface;


	public override void OnTurnStart () {
		CombatPlayerInterface.ShowActionList(Combatant.ActionList);
	}

	public override async Task<ICombatant> RequestSingleTarget (ICombatant user, TargetSelector selector) {
		var selectables = Battle.Current.Combatants;

		var predicates = new List<Predicate<ICombatant>> ();

		if (selector.Side != null) predicates.Add(x => (int) x.CombatPosition.Side * (int) selector.Side == (int) user.CombatPosition.Side);
		if (selector.Row != null) predicates.Add(x => x.CombatPosition.Row == selector.Row);
		if (selector.Validator != null) predicates.Add(selector.Validator);

		selectables = selectables.Where(combatant => {
			foreach (var predicate in predicates) {
				if (!predicate(combatant)) return false;
			}

			return true;
		}).ToList();

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

	public override async Task<CombatPosition> RequestPosition () {
		var available_positions = Battle.Positioner.GetAvailablePositions();

		foreach (var position in available_positions) {
			var marker = SingleMarkerScene.Instantiate<KeyboardSelectionMarker>();
			marker.Position = Battle.Positioner.GetWorldPosition(position);
			marker.OnCombatantSelected = () => {
				Dev.Log($"POSITION CLICKED");
			};
			AddChild(marker);
		}

		return new CombatPosition() { Side = Combatant.CombatPosition.Side, Row = 1, Slot = 2, };
	}
}
