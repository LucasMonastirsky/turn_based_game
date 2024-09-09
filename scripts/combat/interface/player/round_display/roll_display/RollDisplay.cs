using System.Collections.Generic;
using Combat;
using Development;
using Godot;

public partial class RollDisplay : Control {
	[Export] Container LabelContainer;

	private List<Label> Labels = new ();

	private static RollDisplay Current;

	public RollDisplay () {
		Dev.Log("RollDisplay constructor");
		Current = this;
	}

	public override void _EnterTree () {

	}

	public static void Clear () {
		Current.Labels.ForEach(label => label.QueueFree());
		Current.Labels = new ();
	}

	public static void ShowRoll (RollResult roll_result, int? target = null) {
		if (roll_result.Combatant is null) return;

		var label = new Label ();
		Current.LabelContainer.AddChild(label);
		Current.Labels.Add(label);

		label.Text = $"{roll_result.Combatant.Name} rolled {roll_result.Stat}: {roll_result.Total}{(target is null ? "" : $"/{target}")} ({roll_result.DiceValue} + {roll_result.Bonus})";
	}
}
