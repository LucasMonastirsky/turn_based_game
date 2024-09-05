using Combat;
using Godot;
using System;
using System.Collections.Generic;

public partial class CombatantDetail : HBoxContainer {
	private static CombatantDetail Current;

	[Export] private TextureRect Icon;
	[Export] private Label LabelName;
	[Export] private Container StatLabelContainer;
	[Export] PackedScene StatLabelScene;

	private List<StatLabel> StatLabels = new ();

	private static Combatant _combatant;
	public static Combatant Combatant {
		get => _combatant;
		set {
			_combatant = value;
			Current.Icon.Texture = value.Icon;
			Current.LabelName.Text = value.Name;
			Current.StatLabels.ForEach(label => label.Combatant = value);
		}
	}

	public override void _EnterTree () {
		Current = this;
	}
}
