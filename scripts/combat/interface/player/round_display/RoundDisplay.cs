using Combat;
using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class RoundDisplay : Control {
	private static RoundDisplay Current;

	private List<RoundDisplayIcon> Icons;
	private List<RoundDisplaySwapButton> SwapButtons;

	public RoundDisplay () {
		Current = this;
	}

	public override void _EnterTree () {
		var children = GetChildren()[0].GetChildren()[0].GetChildren();

		Icons = children.Where(child => child is RoundDisplayIcon).Select(child => child as RoundDisplayIcon).ToList();
		SwapButtons = children.Where(child => child is RoundDisplaySwapButton).Select(child => child as RoundDisplaySwapButton).ToList();
	}

	public static void UpdateIcons () {
		for (var i = 0; i < Current.Icons.Count; i++) {
			Current.Icons[i].Texture = RoundManager.GetCombatant(i)?.Icon;
		}
	}
}
