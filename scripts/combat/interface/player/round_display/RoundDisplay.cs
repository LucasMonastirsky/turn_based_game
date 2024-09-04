using Combat;
using Development;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class RoundDisplay : HBoxContainer {
	private static RoundDisplay Current;

	private List<RoundDisplayIcon> Icons;
	private List<RoundDisplaySwapButton> SwapButtons;

	public RoundDisplay () {
		Current = this;
	}

	public override void _EnterTree () {
		var children = GetChildren();

		foreach (var child in children) {
			Dev.Log($"{child.Name}: {child is RoundDisplayIcon}/{child is RoundDisplaySwapButton}");
		}

		Icons = children.Where(child => child is RoundDisplayIcon).Select(child => child as RoundDisplayIcon).ToList();
		SwapButtons = children.Where(child => child is RoundDisplaySwapButton).Select(child => child as RoundDisplaySwapButton).ToList();
	}

	public static void UpdateIcons () {
		for (var i = 0; i < Current.Icons.Count; i++) {
			Current.Icons[i].Texture = RoundManager.GetCombatant(i)?.Icon;
		}
	}
}
