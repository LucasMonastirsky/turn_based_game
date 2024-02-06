using System.Collections.Generic;
using Combat;
using Development;
using Godot;

public partial class CombatantDisplayManager : Node {
	private static CombatantDisplayManager instance;
	private static List<CombatantDisplay> displays = new ();

	public override void _Ready () {
		instance = this;
	}

	public static CombatantDisplay CreateDisplay (Combatant combatant) {
		var display = new CombatantDisplay () { User = combatant };
		instance.AddChild(display);
		displays.Add(display);
		return display;
	}

	public static void Hide () {
		foreach (var display in displays) {
			display.Visible = false;
		}
	}

	public static void Show () {
		foreach (var display in displays) {
			display.Visible = true;
		}
	}
}
