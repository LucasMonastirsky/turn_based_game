using Combat;
using Godot;
using System;

public partial class DebugInterface : Node2D {
	Label label_turn_manager, label_interaction_manager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		label_turn_manager = new () {
			Name = "LabelTurnManager",
			Position = new (0, -100),
		};
		AddChild(label_turn_manager);

		label_interaction_manager = new () {
			Name = "LabelTurnManager",
			Position = new (0, -50),
		};
		AddChild(label_interaction_manager);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		label_turn_manager.Text = $"{TurnManager.State}";
		//label_interaction_manager.Text = $"{InteractionManager.State}: {InteractionManager.CurrentAction}";
	}
}
